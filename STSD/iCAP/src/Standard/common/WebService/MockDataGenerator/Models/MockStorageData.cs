using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockDataCreate.Models
{
    public class MockStorageData
    {
        public string StorageSN { get; set; }
        public double[] HealthList { get; set; }
        public int[] AvgECList { get; set; }
        const int LIFESPAN_DATA_COUNT = 20;
        public MockStorageData Mock(int index, Random rnd)
        {
            int[] data;
            double[] health = new double[20];
            int[] AvgEC = new int[20];

            if (index % 5 == 0)
            {
                data = Enumerable.Repeat(0, 20).Select(j => rnd.Next(20, 2450)).ToArray();
            }
            else if (index % 2 == 0)
            {
                data = Enumerable.Repeat(0, 20).Select(j => rnd.Next(1800, 2450)).ToArray();
            }
            else
            {
                data = Enumerable.Repeat(0, 20).Select(j => rnd.Next(2100, 2450)).ToArray();
            }

            Array.Sort(data);
            Array.Reverse(data);

            Parallel.For(0, LIFESPAN_DATA_COUNT, k =>
            {
                health[k] = data[k] / 2450.0 * 100.0;
                AvgEC[k] = 3000 - (int)Math.Floor(30 * health[k]);//1% = 30
            });

            return new MockStorageData()
            {
                StorageSN = "BCADevice" + (index).ToString("D5"),
                HealthList = health,
                AvgECList = AvgEC
            };
        }
    }
}
