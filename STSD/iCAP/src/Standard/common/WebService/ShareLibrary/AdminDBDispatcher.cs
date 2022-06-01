using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.AdminDB;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Threading.Tasks;
using ShareLibrary.DataTemplate;
using ShareLibrary.Interface;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using ShareLibrary.AbstractServices;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net;
using System.Globalization;

namespace ShareLibrary
{
    public class AdminDBDispatcher
    {
        public AdminDBDispatcher(DefaultSQLData _DefaultSQLData) {
            DefaultSQLData = _DefaultSQLData;
        }
        DefaultSQLData DefaultSQLData;

#if DFI_RELEASE
        string[] DefaultDataGroupList = { "System", "CPU", "Motherboard", "Memory", "Storage", "NET", "External", "GPU", "EAPI" };
#else
        string[] DefaultDataGroupList = { "System", "CPU", "Motherboard", "Memory", "Storage", "NET", "External", "GPU" };
#endif
        Data[] DefaultDataList =
        {
            new Data(){Id = 1, Name = "OS name", GroupId = 1, Location = "{\"Static\":{\"Sys\":{\"OS\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 2, Name = "OS version", GroupId = 1, Location = "{\"Static\":{\"Sys\":{\"OSVer\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 3, Name = "OS architecture", GroupId = 1, Location = "{\"Static\":{\"Sys\":{\"OSArch\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 4, Name = "Computer name", GroupId = 1, Location = "{\"Static\":{\"Sys\":{\"Name\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 5, Name = "Device longitude", GroupId = 1, Location = "{\"Static\":{\"Sys\":{\"Longitude\":0}}}", Numberical = true, Dynamic = false},
            new Data(){Id = 6, Name = "Device latitude", GroupId = 1, Location = "{\"Static\":{\"Sys\":{\"Latitude\":0}}}", Numberical = true, Dynamic = false},
            new Data(){Id = 7, Name = "CPU manufacturer", GroupId = 2, Location = "{\"Static\":{\"CPU\":{\"Manu\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 8, Name = "CPU name", GroupId = 2, Location = "{\"Static\":{\"CPU\":{\"Name\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 9, Name = "Number of CPU core", GroupId = 2, Location = "{\"Static\":{\"CPU\":{\"Numofcore\":0}}}", Numberical = true, Dynamic = false, Unit = "Core"},
            new Data(){Id = 10, Name = "CPU L2 cache size", GroupId = 2, Location = "{\"Static\":{\"CPU\":{\"L2\":0}}}", Numberical = true, Dynamic = false, Unit="KB"},
            new Data(){Id = 11, Name = "CPU L3 cache size", GroupId = 2, Location = "{\"Static\":{\"CPU\":{\"L3\":0}}}", Numberical = true, Dynamic = false, Unit = "KB"},
            new Data(){Id = 12, Name = "CPU frequency", GroupId = 2, Location = "{\"Dynamic\":{\"CPU\":{\"Freq\":0}}}", Numberical = true, Dynamic = true, Unit = "MHz", EventMessage = "CPU frequency over thershold, value : "},
            new Data(){Id = 13, Name = "CPU loading", GroupId = 2, Location = "{\"Dynamic\":{\"CPU\":{\"Usage\":0}}}", Numberical = true, Dynamic = true, Unit = "%", EventMessage = "CPU usage over thershold, value : "},
            new Data(){Id = 14, Name = "CPU fan rotating speed", GroupId = 2, Location = "{\"Dynamic\":{\"CPU\":{\"FanRPM\":0}}}", Numberical = true, Dynamic = true, Unit = "RPM", EventMessage = "CPU fan rotating speed over thershold, value : "},
            new Data(){Id = 15, Name = "CPU each core frequency", GroupId = 2, Location = "{\"Dynamic\":{\"CPU\":{\"0\":{\"Freq\":0}}}}", Numberical = false, Dynamic = false, Unit = "MHz", EventMessage = "CPU core X frequency over thershold, value : "},
            new Data(){Id = 16, Name = "CPU each core usage", GroupId = 2, Location = "{\"Dynamic\":{\"CPU\":{\"0\":{\"Usage\":0}}}}", Numberical = false, Dynamic = false, Unit = "%", EventMessage = "CPU core X usage over thershold, value : "},
            new Data(){Id = 17, Name = "CPU each core temperature", GroupId = 2, Location = "{\"Dynamic\":{\"CPU\":{\"0\":{\"Temp\":0}}}}", Numberical = false, Dynamic = false, Unit = "°C", EventMessage = "CPU core X temperature over thershold, value : "},
            new Data(){Id = 18, Name = "CPU each core voltage", GroupId = 2, Location = "{\"Dynamic\":{\"CPU\":{\"0\":{\"V\":0}}}}", Numberical = false, Dynamic = false, Unit = "mV", EventMessage = "CPU core X voltage over thershold, value : "},
            new Data(){Id = 19, Name = "Motherboard manufacturer", GroupId = 3, Location = "{\"Static\":{\"MB\":{\"Manu\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 20, Name = "Motherboard product" , GroupId = 3, Location = "{\"Static\":{\"MB\":{\"Product\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 21, Name = "Motherboard serial number", GroupId = 3, Location = "{\"Static\":{\"MB\":{\"SN\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 22, Name = "BIOS manufacturer", GroupId = 3, Location = "{\"Static\":{\"MB\":{\"BIOSManu\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 23, Name = "BIOS version", GroupId = 3, Location = "{\"Static\":{\"MB\":{\"BIOSVer\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 24, Name = "Memory total capacity", GroupId = 4, Location = "{\"Static\":{\"MEM\":{\"Cap\":0}}}", Numberical = true, Dynamic = false, Unit = "KB"},
            new Data(){Id = 25, Name = "Memory manufacturer", GroupId = 4, Location = "{\"Static\":{\"MEM\":{\"0\":{\"Manu\":0}}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 26, Name = "Memory location", GroupId = 4, Location = "{\"Static\":{\"MEM\":{\"0\":{\"Loc\":0}}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 27, Name = "Memory capacity", GroupId = 4, Location = "{\"Static\":{\"MEM\":{\"0\":{\"Cap\":0}}}}", Numberical = false, Dynamic = false, Unit = "KB"},
            new Data(){Id = 28, Name = "Memory frequency", GroupId = 4, Location = "{\"Static\":{\"MEM\":{\"0\":{\"Freq\":0}}}}", Numberical = false, Dynamic = false, Unit = "MHz"},
            new Data(){Id = 29, Name = "Memory voltage", GroupId = 4, Location = "{\"Static\":{\"MEM\":{\"0\":{\"V\":0}}}}", Numberical = false, Dynamic = false, Unit = "V"},
            new Data(){Id = 30, Name = "Memory used size", GroupId = 4, Location = "{\"Dynamic\":{\"MEM\":{\"memUsed\":0}}}", Numberical = true, Dynamic = true, Unit = "KB", EventMessage = "Memory unused size over thershold, value : "},
            new Data(){Id = 31, Name = "Storage model", GroupId = 5, Location = "{\"Static\":{\"Storage\":[{\"Model\":0}]}}", Numberical = false, Dynamic = false},
            new Data(){Id = 32, Name = "Storage serial number", GroupId = 5, Location = "{\"Static\":{\"Storage\":[{\"SN\":0}]}}", Numberical = false, Dynamic = false},
            new Data(){Id = 33, Name = "Storage firmware version", GroupId = 5, Location = "{\"Static\":{\"Storage\":[{\"FWVer\":0}]}}", Numberical = false, Dynamic = false},
            new Data(){Id = 34, Name = "Storage total capacity", GroupId = 5, Location = "{\"Static\":{\"Storage\":[{\"Par\":{\"TotalCap\":0}}]}}", Numberical = true, Dynamic = false, Unit = "KB"},
            new Data(){Id = 35, Name = "Number of storage partition", GroupId = 5, Location = "{\"Static\":{\"Storage\":[{\"Par\":{\"NumofPar\":0}}]}}", Numberical = true, Dynamic = false},
            new Data(){Id = 36, Name = "Storage partition location", GroupId = 5, Location = "{\"Static\":{\"Storage\":[{\"Par\":{\"ParInfo\":[{\"MountAt\":0}]}}]}}", Numberical = false, Dynamic = false},
            new Data(){Id = 37, Name = "Storage partition capacity", GroupId = 5, Location = "{\"Static\":{\"Storage\":[{\"Par\":{\"ParInfo\":[{\"Capacity\":0}]}}]}}", Numberical = true, Dynamic = false,  Unit = "KB"},
            new Data(){Id = 38, Name = "Storage power on hours", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"smart\":{\"9\":0}}]}}", Numberical = true, Dynamic = true, Unit = "h", EventMessage = "Storage X power on hours over thershold, value : "},
            new Data(){Id = 39, Name = "Storage power on cycles", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"smart\":{\"12\":0}}]}}", Numberical = true, Dynamic = true, Unit = "times", EventMessage = "Storage X power on cycles over thershold, value : "},
            new Data(){Id = 40, Name = "Storage average erase count", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"smart\":{\"167\":0}}]}}", Numberical = true, Dynamic = true, Unit = "times", EventMessage = "Storage X average erase count over thershold, value : "},
            new Data(){Id = 41, Name = "Storage temperature", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"smart\":{\"194\":0}}]}}", Numberical = true, Dynamic = true, Unit = "°C", EventMessage = "Storage X temperature over thershold, value : "},
            new Data(){Id = 42, Name = "Storage health", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"Health\":0}]}}", Numberical = true, Dynamic = true, Unit = "%", EventMessage = "Storage X health over thershold, value : "},
            new Data(){Id = 43, Name = "Storage PECycle", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"PECycle\":0}]}}", Numberical = true, Unit = "times", Dynamic = false},
            new Data(){Id = 44, Name = "Storage sequential read count (total)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SRC\":0}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (total) over thershold, value : "},
            new Data(){Id = 45, Name = "Storage random read count (total)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"RRC\":0}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (total) over thershold, value : "},
            new Data(){Id = 46, Name = "Storage sequential write count (total)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SWC\":0}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (total) over thershold, value : "},
            new Data(){Id = 47, Name = "Storage random write count (total)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"RWC\":0}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random write count (total) over thershold, value : "},
            new Data(){Id = 48, Name = "Storage sequential read count (8.xM)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SR\":{\"0\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (8.xM) over thershold, value : "},
            new Data(){Id = 49, Name = "Storage sequential read count (4.xM)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SR\":{\"1\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (4.xM) over thershold, value : "},
            new Data(){Id = 50, Name = "Storage sequential read count (1.xM)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SR\":{\"2\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (1.xM) over thershold, value : "},
            new Data(){Id = 51, Name = "Storage sequential read count (128.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SR\":{\"3\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (128.xK) over thershold, value : "},
            new Data(){Id = 52, Name = "Storage sequential read count (64.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SR\":{\"4\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (64.xK) over thershold, value : "},
            new Data(){Id = 53, Name = "Storage sequential read count (32.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SR\":{\"5\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (32.xK) over thershold, value : "},
            new Data(){Id = 54, Name = "Storage sequential write count (8.xM)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SW\":{\"0\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (8.xM) over thershold, value : "},
            new Data(){Id = 55, Name = "Storage sequential write count (4.xM)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SW\":{\"1\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (4.xM) over thershold, value : "},
            new Data(){Id = 56, Name = "Storage sequential write count (1.xM)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SW\":{\"2\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (1.xM) over thershold, value : "},
            new Data(){Id = 57, Name = "Storage sequential write count (128.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SW\":{\"3\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (128.xK) over thershold, value : "},
            new Data(){Id = 58, Name = "Storage sequential write count (64.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SW\":{\"4\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (64.xK) over thershold, value : "},
            new Data(){Id = 59, Name = "Storage sequential write count (32.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"SW\":{\"5\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (32.xK) over thershold, value : "},
            new Data(){Id = 60, Name = "Storage random read count (64.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"RR\":{\"0\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (64.xK) over thershold, value : "},
            new Data(){Id = 61, Name = "Storage random read count (32.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"RR\":{\"1\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (32.xK) over thershold, value : "},
            new Data(){Id = 62, Name = "Storage random read count (16.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"RR\":{\"2\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (16.xK) over thershold, value : "},
            new Data(){Id = 63, Name = "Storage random read count (8.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"RR\":{\"3\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (8.xK) over thershold, value : "},
            new Data(){Id = 64, Name = "Storage random read count (4.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"RR\":{\"4\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (4.xK) over thershold, value : "},
            new Data(){Id = 65, Name = "Storage random write count (64.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"RW\":{\"0\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random write count (64.xK) over thershold, value : "},
            new Data(){Id = 66, Name = "Storage random write count (32.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"RW\":{\"1\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random write count (32.xK) over thershold, value : "},
            new Data(){Id = 67, Name = "Storage random write count (16.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"RW\":{\"2\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random write count (16.xK) over thershold, value : "},
            new Data(){Id = 68, Name = "Storage random write count (8.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"RW\":{\"3\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random write count (8.xK) over thershold, value : "},
            new Data(){Id = 69, Name = "Storage random write count (4.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":1,\"RW\":{\"4\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X Storage random write count (4.xK) over thershold, value : "},
            new Data(){Id = 70, Name = "Storage sequential read count (128.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SR\":{\"0\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (128.xK) over thershold, value : "},
            new Data(){Id = 71, Name = "Storage sequential read count (64.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SR\":{\"1\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (64.xK) over thershold, value : "},
            new Data(){Id = 72, Name = "Storage sequential read count (32.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SR\":{\"2\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (32.xK) over thershold, value : "},
            new Data(){Id = 73, Name = "Storage sequential read count (16.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SR\":{\"3\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (16.xK) over thershold, value : "},
            new Data(){Id = 74, Name = "Storage sequential read count (8.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SR\":{\"4\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (8.xK) over thershold, value : "},
            new Data(){Id = 75, Name = "Storage sequential read count (4.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SR\":{\"5\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (4.xK) over thershold, value : "},
            new Data(){Id = 76, Name = "Storage sequential read count (0.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SR\":{\"6\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential read count (0.xK) over thershold, value : "},
            new Data(){Id = 77, Name = "Storage sequential write count (128.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SW\":{\"0\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (128.xK) over thershold, value : "},
            new Data(){Id = 78, Name = "Storage sequential write count (64.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SW\":{\"1\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (64.xK) over thershold, value : "},
            new Data(){Id = 79, Name = "Storage sequential write count (32.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SW\":{\"2\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (32.xK) over thershold, value : "},
            new Data(){Id = 80, Name = "Storage sequential write count (16.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SW\":{\"3\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (16.xK) over thershold, value : "},
            new Data(){Id = 81, Name = "Storage sequential write count (8.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SW\":{\"4\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (8.xK) over thershold, value : "},
            new Data(){Id = 82, Name = "Storage sequential write count (4.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SW\":{\"5\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (4.xK) over thershold, value : "},
            new Data(){Id = 83, Name = "Storage sequential write count (0.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"SW\":{\"6\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X sequential write count (0.xK) over thershold, value : "},
            new Data(){Id = 84, Name = "Storage random read count (128.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RR\":{\"0\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (128.xK) over thershold, value : "},
            new Data(){Id = 85, Name = "Storage random read count (64.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RR\":{\"1\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (64.xK) over thershold, value : "},
            new Data(){Id = 86, Name = "Storage random read count (32.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RR\":{\"2\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (32.xK) over thershold, value : "},
            new Data(){Id = 87, Name = "Storage random read count (16.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RR\":{\"3\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (16.xK) over thershold, value : "},
            new Data(){Id = 88, Name = "Storage random read count (8.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RR\":{\"4\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (8.xK) over thershold, value : "},
            new Data(){Id = 89, Name = "Storage random read count (4.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RR\":{\"5\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (4.xK) over thershold, value : "},
            new Data(){Id = 90, Name = "Storage random read count (0.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RR\":{\"6\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random read count (0.xK) over thershold, value : "},
            new Data(){Id = 91, Name = "Storage random write count (128.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RW\":{\"0\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random write count (128.xK) over thershold, value : "},
            new Data(){Id = 92, Name = "Storage random write count (64.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RW\":{\"1\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random write count (64.xK) over thershold, value : "},
            new Data(){Id = 93, Name = "Storage random write count (32.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RW\":{\"2\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random write count (32.xK) over thershold, value : "},
            new Data(){Id = 94, Name = "Storage random write count (16.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RW\":{\"3\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random write count (16.xK) over thershold, value : "},
            new Data(){Id = 95, Name = "Storage random write count (8.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RW\":{\"4\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random write count (8.xK) over thershold, value : "},
            new Data(){Id = 96, Name = "Storage random write count (4.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RW\":{\"5\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random write count (4.xK) over thershold, value : "},
            new Data(){Id = 97, Name = "Storage random write count (0.xK)", GroupId = 5, Location = "{\"Dynamic\":{\"Storage\":[{\"iAnalyzer\":{\"Enable\":2,\"RW\":{\"6\":0}}}]}}", Numberical = false, Dynamic = false, EventMessage = "Storage X random write count (0.xK) over thershold, value : "},
            new Data(){Id = 98, Name = "Storage estimation lifespan", GroupId = 5, Location = "{\"StorageAnalyzer\":{\"Lifespan\":[{\"data\":0}]}}", Numberical = true, Dynamic = true, Unit = "days", EventMessage = "Storage X lifespan over thershold, value : "},
            new Data(){Id = 99, Name = "Network card name", GroupId = 6, Location = "{\"Static\":{\"Net\":[{\"Name\":0}]}}", Numberical = false, Dynamic = false},
            new Data(){Id = 100, Name = "Network card type", GroupId = 6, Location = "{\"Static\":{\"Net\":[{\"Type\":0}]}}", Numberical = false, Dynamic = false},
            new Data(){Id = 101, Name = "Network card MAC address", GroupId = 6, Location = "{\"Static\":{\"Net\":[{\"MAC\":0}]}}", Numberical = false, Dynamic = false},
            new Data(){Id = 102, Name = "Network card IP address", GroupId = 6, Location = "{\"Static\":{\"Net\":[{\"IPAddr\":0}]}}", Numberical = false, Dynamic = false},
            new Data(){Id = 103, Name = "Network card IPv6 address", GroupId = 6, Location = "{\"Static\":{\"Net\":[{\"IPv6\":0}]}}", Numberical = false, Dynamic = false},
            new Data(){Id = 104, Name = "Network card netmask", GroupId = 6, Location = "{\"Static\":{\"Net\":[{\"Netmask\":0}]}}", Numberical = false, Dynamic = false, EventMessage = "X offline"},
            new Data(){Id = 105, Name = "Device status", GroupId = 1, Location = "{\"Redis\":{\"DB1\":0}}", Numberical = true, Dynamic = true},
            new Data(){Id = 106, Name = "GPU name", GroupId = 8, Location = "{\"Static\":{\"GPU\":{\"Name\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 107, Name = "GPU architecture", GroupId = 8, Location = "{\"Static\":{\"GPU\":{\"Arch\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 108, Name = "GPU CUDA driver version", GroupId = 8, Location = "{\"Static\":{\"GPU\":{\"DriverVer\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 109, Name = "GPU compute capability", GroupId = 8, Location = "{\"Static\":{\"GPU\":{\"ComputeCap\":0}}}", Numberical = true, Dynamic = false},
            new Data(){Id = 110, Name = "GPU CUDA cores", GroupId = 8, Location = "{\"Static\":{\"GPU\":{\"CoreNum\":0}}}", Numberical = true, Dynamic = false},
            new Data(){Id = 111, Name = "GPU clock ", GroupId = 8, Location = "{\"Static\":{\"GPU\":{\"Clock\":0}}}", Numberical = false, Dynamic = false, Unit = "MHz"},
            new Data(){Id = 112, Name = "GPU memory type", GroupId = 8, Location = "{\"Static\":{\"GPU\":{\"MemType\":0}}}", Numberical = false, Dynamic = false},
            new Data(){Id = 113, Name = "GPU memory bus width", GroupId = 8, Location = "{\"Static\":{\"GPU\":{\"MemBusWidth\":0}}}", Numberical = false, Dynamic = false, Unit = "bit"},
            new Data(){Id = 114, Name = "GPU memory size", GroupId = 8, Location = "{\"Static\":{\"GPU\":{\"MemSize\":0}}}", Numberical = false, Dynamic = false, Unit = "MB"},
            new Data(){Id = 115, Name = "GPU memory bandwitdh ", GroupId = 8, Location = "{\"Static\":{\"GPU\":{\"MemBandWidth\":0}}}", Numberical = false, Dynamic = false, Unit = "GB/s"},
            new Data(){Id = 116, Name = "GPU memory clock ", GroupId = 8, Location = "{\"Static\":{\"GPU\":{\"MemClock\":0}}}", Numberical = false, Dynamic = false, Unit = "MHz"},
            new Data(){Id = 117, Name = "GPU core clock", GroupId = 8, Location = "{\"Dynamic\":{\"GPU\":{\"CoreClock\":0}}}", Numberical = true, Dynamic = true, Unit = "MHz"},
            new Data(){Id = 118, Name = "GPU temperature", GroupId = 8, Location = "{\"Dynamic\":{\"GPU\":{\"Temp\":0}}}", Numberical = true, Dynamic = true, Unit = "℃"},
            new Data(){Id = 119, Name = "GPU loading", GroupId = 8, Location = "{\"Dynamic\":{\"GPU\":{\"Load\":0}}}", Numberical = true, Dynamic = true, Unit = "%"},
            new Data(){Id = 120, Name = "GPU fan temperature", GroupId = 8, Location = "{\"Dynamic\":{\"GPU\":{\"FanTemp\":0}}}", Numberical = true, Dynamic = true, Unit = "℃"},
            new Data(){Id = 121, Name = "GPU memory used", GroupId = 8, Location = "{\"Dynamic\":{\"GPU\":{\"MemUsed\":0}}}", Numberical = true, Dynamic = true, Unit = "%"},
            new Data(){Id = 122, Name = "InnoAGE status", GroupId = 1, Location = "{\"Redis\":{\"DB2\":0}}", Numberical = true, Dynamic = true},

#if DFI_RELEASE
            new Data(){Id = 123, Name = "VCORE", GroupId = 9, Location = "{\"Dynamic\":{\"EAPI\":{\"HWMON\":{\"VOLTAGE\":{\"VCORE\":0}}}}}", Numberical = true, Dynamic = true, Unit = "V"},
            new Data(){Id = 124, Name = "VBAT", GroupId = 9, Location = "{\"Dynamic\":{\"EAPI\":{\"HWMON\":{\"VOLTAGE\":{\"VBAT\":0}}}}}", Numberical = true, Dynamic = true, Unit = "V"},
            new Data(){Id = 125, Name = "5V", GroupId = 9, Location = "{\"Dynamic\":{\"EAPI\":{\"HWMON\":{\"VOLTAGE\":{\"5V\":0}}}}}", Numberical = true, Dynamic = true, Unit = "V"},
            new Data(){Id = 126, Name = "+12V", GroupId = 9, Location = "{\"Dynamic\":{\"EAPI\":{\"HWMON\":{\"VOLTAGE\":{\"12V\":0}}}}}", Numberical = true, Dynamic = true, Unit = "V"},
            new Data(){Id = 127, Name = "3VSB", GroupId = 9, Location = "{\"Dynamic\":{\"EAPI\":{\"HWMON\":{\"VOLTAGE\":{\"3VSB\":0}}}}}", Numberical = true, Dynamic = true, Unit = "V"},
            new Data(){Id = 128, Name = "VDDQ", GroupId = 9, Location = "{\"Dynamic\":{\"EAPI\":{\"HWMON\":{\"VOLTAGE\":{\"VDDQ\":0}}}}}", Numberical = true, Dynamic = true, Unit = "V"},
            new Data(){Id = 129, Name = "CPUTEMP", GroupId = 9, Location = "{\"Dynamic\":{\"EAPI\":{\"HWMON\":{\"TEMP\":{\"CPUTEMP\":0}}}}}", Numberical = true, Dynamic = true, Unit = "℃"},
            new Data(){Id = 130, Name = "SYSTEMP", GroupId = 9, Location = "{\"Dynamic\":{\"EAPI\":{\"HWMON\":{\"TEMP\":{\"SYSTEMP\":0}}}}}", Numberical = true, Dynamic = true, Unit = "℃"},
            new Data(){Id = 131, Name = "CPUFAN", GroupId = 9, Location = "{\"Dynamic\":{\"EAPI\":{\"HWMON\":{\"FAN\":{\"CPUFAN\":0}}}}}", Numberical = true, Dynamic = true, Unit = "RPM"},
            new Data(){Id = 132, Name = "SYSFAN", GroupId = 9, Location = "{\"Dynamic\":{\"EAPI\":{\"HWMON\":{\"FAN\":{\"SYSFAN\":0}}}}}", Numberical = true, Dynamic = true, Unit = "RPM"},
            new Data(){Id = 133, Name = "SYS2FAN", GroupId = 9, Location = "{\"Dynamic\":{\"EAPI\":{\"HWMON\":{\"FAN\":{\"SYS2FAN\":0}}}}}", Numberical = true, Dynamic = true, Unit = "RPM"},
#endif
            };

        Chart[] DefaultChartList = new Chart[]
        {
            //111 mean large medium small
            new Chart(){ Id = (int)DataDefine.DataId.CHART_BAR, Name="bar", SizeFlag = (int)DataDefine.Width.SIZE_1X1 | (int)DataDefine.Width.SIZE_1X2 | (int)DataDefine.Width.SIZE_1X3, Type = Convert.ToInt32("011",2)},
            new Chart(){ Id = (int)DataDefine.DataId.CHART_DOUGHNUT, Name="doughnut", SizeFlag = (int)DataDefine.Width.SIZE_1X1 | (int)DataDefine.Width.SIZE_1X2, Type = Convert.ToInt32("011",2)},
            new Chart(){ Id = (int)DataDefine.DataId.CHART_GAUGE, Name="gauge", SizeFlag = (int)DataDefine.Width.SIZE_1X1 | (int)DataDefine.Width.SIZE_1X2, Type = Convert.ToInt32("010",2)},
            new Chart(){ Id = (int)DataDefine.DataId.CHART_GoogleMap, Name="google map", SizeFlag = (int)DataDefine.Width.SIZE_1X1 | (int)DataDefine.Width.SIZE_1X2 | (int)DataDefine.Width.SIZE_1X3, Type = Convert.ToInt32("111",2)},
            new Chart(){ Id = (int)DataDefine.DataId.CHART_PIE, Name="pie", SizeFlag = (int)DataDefine.Width.SIZE_1X1 | (int)DataDefine.Width.SIZE_1X2, Type = Convert.ToInt32("011",2)},
            new Chart(){ Id = (int)DataDefine.DataId.CHART_TEXT, Name="text", SizeFlag = (int)DataDefine.Width.SIZE_1X1, Type = Convert.ToInt32("001",2)},
            new Chart(){ Id = (int)DataDefine.DataId.CHART_VECTORMAP, Name="vector map", SizeFlag = (int)DataDefine.Width.SIZE_1X1 | (int)DataDefine.Width.SIZE_1X2 | (int)DataDefine.Width.SIZE_1X3, Type = Convert.ToInt32("111",2)},
            new Chart(){ Id = (int)DataDefine.DataId.CHART_OPENSTREETMAP, Name="open street map", SizeFlag = (int)DataDefine.Width.SIZE_1X1 | (int)DataDefine.Width.SIZE_1X2 | (int)DataDefine.Width.SIZE_1X3, Type = Convert.ToInt32("111",2)},
            new Chart(){ Id = (int)DataDefine.DataId.CHART_GAODEMAP, Name="gaode map", SizeFlag = (int)DataDefine.Width.SIZE_1X1 | (int)DataDefine.Width.SIZE_1X2 | (int)DataDefine.Width.SIZE_1X3, Type = Convert.ToInt32("111",2)},
            new Chart(){ Id = (int)DataDefine.DataId.CHART_CUSTOMIZEDMAP, Name="customized map", SizeFlag = (int)DataDefine.Width.SIZE_3X3, Type = Convert.ToInt32("001",2)},
        };

        const byte MORMAL_MODE = 0;
        private AdminDB.Threshold[] defaultThreshold = new AdminDB.Threshold[]
        {
            new AdminDB.Threshold()
            {
                Id = 1,
                DataId = 41, //storage temperatute
                DenominatorId = null,
                Action = 1,
                DeletedFlag = false,
                Func = 2,
                Name = "Storage Temperature",
                Value = "50",
                Enable = false,
                Mode = MORMAL_MODE,
                
            },
            new AdminDB.Threshold()
            {
                Id = 2,
                DataId = 98,
                DenominatorId = null,
                Action = 1,
                DeletedFlag = false,
                Func = 1,
                Name = "Storage Lifespan",
                Value = "150",
                Enable = false,
                Mode = MORMAL_MODE,
            }
        };

        private ThresholdBranchList[] DefaultThresholdBranchList = new ThresholdBranchList[]
        {
            new ThresholdBranchList()
            {
                Id = 1,
                BranchId = 1,
                DeletedFlag = false,
                ThresholdId = 1,
            },
            new ThresholdBranchList()
            {
                Id = 2,
                BranchId = 1,
                DeletedFlag = false,
                ThresholdId = 2,
            },
        };
        public void InitialCheck()
        {
            icapContext ic = new icapContext();
            //ic.Database.EnsureCreated();
            int DelayTime = 100;
            do
            {
                try
                {
                    ic.Database.EnsureCreated();
                    break;
                }
                catch (MySqlException)
                {
                    try
                    {
                        Task.Delay(DelayTime);
                    }
                    catch (Exception)
                    {
                        DelayTime = 50;
                    }
                    DelayTime *= 2;
                }
            } while (true);

            Company co = ic.Company.Find(1);
            if (co == null)
            {
#region Website data
                co = new Company()
                {
                    Id = 1,
                    Name = "default",
                };
                Deviceclass dc = new Deviceclass()
                {
                    Name = "Default",
                    Description = "Default class"
                };
                Permission guest = new Permission()
                {
                    Id = (int)DataDefine.Permission.Id_Guest,
                    Name = DataDefine.PermissionName.GuestName,
                    Level = (int)DataDefine.Permission.Level_Guest,
                    Create = 0b0000000000,
                    Update = 0b0000000000,
                    Delete = 0b0000000000
                };
                Permission admin = new Permission()
                {
                    Id = (int)DataDefine.Permission.Id_Admin,
                    Level = (int)DataDefine.Permission.Level_Admin,
                    Name = DataDefine.PermissionName.AdminName,
                    Create = 0b1111111111,
                    Update = 0b1111111111,
                    Delete = 0b1111111111
                };
                Employee emp = new Employee()
                {
                    Id = (int)DataDefine.Employee.Id_Admin,
                    CompanyId = 1,
                    Password = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin")),
                    LoginName = "admin",
                    LastName = "Administrator",
                    PermissionId = 2,
                    MapId = 5,
                    Email = "",
                    FirstName = "",
                    EmployeeNumber = ""
                };
                Employee emp1 = new Employee()
                {
                    Id = (int)DataDefine.Employee.Id_Guest,
                    CompanyId = 1,
                    Password = Convert.ToBase64String(Encoding.UTF8.GetBytes("guest")),
                    LoginName = "guest",
                    PermissionId = 1,
                    LastName = "Guest",
                    MapId = 5,
                    Email = "",
                    FirstName = "",
                    EmployeeNumber = ""
                };
                LicenseList lic = new LicenseList()
                {
                    DeviceCount = 30,
                    Key = "free",
                    CreatedDate = DateTime.UtcNow
                };


                ic.Company.Add(co);
                ic.Deviceclass.Add(dc);
                ic.LicenseList.Add(lic);
                foreach (Branch b in DefaultSQLData.Branch)
                {
                    ic.Branch.Add(b);
                }
                foreach (Companydashboard d in DefaultSQLData.Dashboard)
                {
                    ic.Companydashboard.Add(d);
                }
                int index = 1;
                foreach (string s in DefaultDataGroupList)
                {
                    DataGroup dg = new DataGroup()
                    {
                        Id = index,
                        Name = s,
                        DeletedFlag = false
                    };
                    ic.DataGroup.Add(dg);
                    index++;
                }
                foreach (Data d in DefaultDataList)
                {
                    ic.Data.Add(d);
                }
                foreach (Chart c in DefaultChartList)
                    ic.Chart.Add(c);
                ic.DeviceRawData.Add(new DeviceRawData() {
                    Id = 1,
                    ExpireDate = 365,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow,
                });
                foreach(Widget widget in DefaultSQLData.Widget)
                {
                    ic.Widget.Add(widget);
                }
                foreach (Companydashboardelement c in DefaultSQLData.DashboardElement)
                {
                    ic.Companydashboardelement.Add(c);
                }
                foreach (WidgetBranchList w in DefaultSQLData.WidgetBranchElement)
                {
                    ic.WidgetBranchList.Add(w);
                }
                foreach (AdminDB.Threshold th in defaultThreshold)
                {
                    ic.Threshold.Add(th);
                }
                foreach (ThresholdBranchList tuple in DefaultThresholdBranchList)
                {
                    ic.ThresholdBranchList.Add(tuple);
                }
                ic.Permission.Add(guest);
                ic.Permission.Add(admin);
                ic.Employee.Add(emp);
                ic.Employee.Add(emp1);
#endregion
                ic.SaveChanges();
            }
        }

        private static double[] stringToDoubleArray(string text, char label)
        {
            return text.Split(label).Select(n => Convert.ToDouble(n)).ToArray();
        }

        private static string[] stringToStringArray(string text, char label)
        {
            return text.Split(label).ToArray();
        }

        private static string GetEmailLabel(string lastName, string firstName, string email)
        {
            var name = (lastName == null || lastName.Length == 0) ? ((firstName == null || firstName.Length == 0) ? "" : firstName) : ((firstName == null || firstName.Length == 0) ? lastName : lastName + " " + firstName);

            return (name == "") ? email : name + $" <{email}>";
        }

        public class _branch : Image, IBranch
        {
            private icapContext ic;
            public _branch()
            {
                ic = new icapContext();
            }
            public SelectOptionTemplate[] GetBranchList()
            {
                return (from a in ic.Branch
                        where a.DeletedFlag == false
                        select new SelectOptionTemplate()
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).DefaultIfEmpty().ToArray();
            }
            public SelectOptionTemplate[] GetBranchList(string devName)
            {
                return (from a in ic.Branch
                        join b in ic.BranchDeviceList on a.Id equals b.BranchId
                        join c in ic.Device on b.DeviceId equals c.Id
                        where c.Name == devName && c.DeletedFlag == false
                        select new SelectOptionTemplate()
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).DefaultIfEmpty().ToArray();
            }

            public bool NameExists(string branchName)
            {
                try
                {
                    var branch = ic.Branch.Where(b => b.Name == branchName && b.DeletedFlag == false).SingleOrDefault();
                    if (branch != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            public bool NameExists(string branchName, int branchId)
            {
                try
                {
                    var branch = ic.Branch.Where(b => b.Name == branchName && b.DeletedFlag == false && b.Id != branchId).SingleOrDefault();
                    if (branch != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            public void Create(string branchName)
            {
                try
                {
                    ic.Branch.Add(new Branch() { Name = branchName, CompanyId = 1});
                    ic.SaveChanges();
                }
                catch
                {
                    if (branchName == null)
                        throw new DbUpdateException("", new Exception("The group name field could NOT be blank!"));
                    throw new DbUpdateException("", new Exception("Failed to create the group. Please refresh the web page and try again."));
                }
            }

            public void Update(BranchTemplate updateBranch)
            {
                if (updateBranch.Name == null)
                    throw new Exception();
                Branch branch = ic.Branch.Where(b => b.Id == updateBranch.Id && b.DeletedFlag == false).SingleOrDefault();
                try
                {
                    branch.LastModifiedDate = DateTime.UtcNow;
                    branch.Name = updateBranch.Name;
                    ic.Branch.Update(branch);
                    ic.SaveChanges();
                }
                catch (Exception)
                {
                    if (branch == null)
                        throw new Exception("The group could NOT be found!");
                    else
                        throw new Exception("Failed to Update the group. Please refresh the web page try again.");
                }
            }
            public void Delete(int branchId)
            {
                if (branchId == 1)
                    throw new Exception("The first group can not be deleted!");
                Branch branch = ic.Branch.Where(b => b.Id == branchId && b.DeletedFlag == false).SingleOrDefault();

                try
                {
                    ic.Database.ExecuteSqlCommand($"DELETE FROM branchdevicelist where BranchId={branchId};");
                    ic.Database.ExecuteSqlCommand($"DELETE FROM widgetbranchlist where BranchId={branchId};");
                    ic.Database.ExecuteSqlCommand($"DELETE FROM thresholdbranchlist where BranchId={branchId};");
                    branch.LastModifiedDate = DateTime.UtcNow;
                    branch.DeletedFlag = true;
                    ic.Branch.Update(branch);
                    ic.SaveChanges();

                    var handler = new BranchChangedHandler("172.30.0.13", "N", "TH/UPDATE", ic, branchId);

                    Thread thread = new Thread(new ThreadStart(handler.CheckAndSend));
                    thread.Start();
                }
                catch (Exception)
                {
                    if (branch == null)
                        throw new Exception("The group could NOT be found!");
                    throw new Exception("Failed to delete the branch! Please refresh the web page and try again.");
                }
            }

            public void DeviceAllocation(DeviceAllocationTemplate deviceAllocation)
            {
                if (deviceAllocation.DeviceIdList.Length == 0)
                    throw new Exception();
                ic.Database.ExecuteSqlCommand($"DELETE FROM branchdevicelist where BranchId={deviceAllocation.BranchId};");
                int[] deviceList_Noduplicate = deviceAllocation.DeviceIdList.Distinct().ToArray();

                foreach (int devId in deviceList_Noduplicate)
                {
                    ic.BranchDeviceList.Add(new BranchDeviceList()
                    {
                        BranchId = deviceAllocation.BranchId,
                        DeviceId = devId,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        DeletedFlag = false

                    });
                }
                ic.SaveChanges();
            }

            public DeviceOption[] GetDeviceList(int branchId)
            {
                var ret = (from a in ic.BranchDeviceList
                           join b in ic.Device on a.DeviceId equals b.Id
                           where a.BranchId == branchId && b.DeletedFlag == false
                           orderby b.Id
                           select new DeviceOption()
                           {
                               DeviceId = a.DeviceId,
                               DeviceName = b.Name
                           }).ToArray();
                if (ret.Length == 0)
                    return new DeviceOption[] { null };
                return ret;
            }

            public BranchTemplate GetInfo(int branchId)
            {
                var ret = (from b in ic.Branch
                           where b.Id == branchId && b.DeletedFlag == false
                           select new BranchTemplate()
                           {
                               Id = b.Id,
                               Name = b.Name,               
                           }).SingleOrDefault();
                if (ret != null)
                {
                    ret.Selected = (from a in ic.Device
                                    join b in ic.BranchDeviceList on a.Id equals b.DeviceId
                                    where a.DeletedFlag == false && b.BranchId == ret.Id
                                    orderby a.Alias
                                    select new SelectOptionTemplate()
                                    {
                                        Id = a.Id,
                                        Name = a.Name,
                                        Alias = a.Alias
                                    }).ToArray();
                    if (ret.Selected.Length == 0)
                    {
                        ret.Selected = null;
                    }
                }

                return ret;
            }

            public class BranchChangedHandler
            {
                string _ip = null;
                string _command = null;
                string _payload = null;
                icapContext _ic = null;
                int _branchId = 0;
                public BranchChangedHandler(string ip, string command, string payload, icapContext ic, int branchId)
                {
                    _ip = ip;
                    _command = command;
                    _payload = payload;
                    _ic = ic;
                    _branchId = branchId;
                }

                public void CheckAndSend()
                {
                    var id = (from a in _ic.ThresholdBranchList
                              where a.BranchId == _branchId
                              select a.Id).FirstOrDefault();

                    if (id > 0)
                    {
                        var notify = new NotifyCoreService(_ip, _command, _payload);
                        notify.Send();
                    }
                }
            }

            public bool SaveInfo(BranchTemplate branchInfo)
            {
                Branch branch = ic.Branch.Where(b => b.Id == branchInfo.Id && b.DeletedFlag == false).SingleOrDefault();

                if (branch == null)
                {
                    return false;
                }

                try
                {
                    BranchDeviceList[] currentSetting = (from a in ic.BranchDeviceList
                                                         where a.BranchId == branchInfo.Id
                                                         select a).ToArray();
                    bool changedFlag = false;
                    if (branchInfo.Selected != null && branchInfo.Selected.Length != 0)
                    {
                        bool[] flag = new bool[branchInfo.Selected.Length];

                        for (int i = 0; i < currentSetting.Length; i++)
                        {
                            int index = Array.IndexOf(branchInfo.Selected, currentSetting[i].DeviceId);

                            if (index != -1)
                            {
                                flag[index] = true;
                            }
                            else
                            {
                                ic.BranchDeviceList.Remove(currentSetting[i]);
                                changedFlag = true;
                            }
                        }

                        for (int i = 0; i < flag.Length; i++)
                        {
                            if (flag[i] == false)
                            {
                                ic.BranchDeviceList.Add(new BranchDeviceList()
                                {
                                    CreatedDate = DateTime.UtcNow,
                                    BranchId = branchInfo.Id,
                                    DeletedFlag = false,
                                    LastModifiedDate = DateTime.UtcNow,
                                    DeviceId = branchInfo.Selected[i].Id,
                                });
                                changedFlag = true;
                            }
                        }
                    }
                    else
                    {
                        ic.Database.ExecuteSqlCommand($"DELETE FROM branchdevicelist where BranchId={branchInfo.Id};");
                        changedFlag = true;
                    }

                    ic.SaveChanges();

                    if (changedFlag)
                    {
                        var handler = new BranchChangedHandler("172.30.0.13", "N", "TH/UPDATE", ic, branchInfo.Id);

                        Thread thread = new Thread(new ThreadStart(handler.CheckAndSend));
                        thread.Start();
                    }
                    return true;
                }
                catch (DbUpdateException exc)
                {
                    throw exc;
                }
                catch (Exception)
                {
                    if (branch == null)
                        throw new Exception("The group could NOT be found!");
                    else
                        throw new Exception("Failed to save the group information. Please refresh the web page and try again.");
                }
            }

            public string GetBranchName(string devName)
            {
                var branchName = (from a in ic.Branch
                                  join b in ic.BranchDeviceList on a.Id equals b.BranchId
                                  join c in ic.Device on b.DeviceId equals c.Id
                                  where c.Name == devName && c.DeletedFlag == false
                                  select a.Name).DefaultIfEmpty().ToArray();
                return string.Join(", ", branchName);

            }            
        }

        public class _device : Image, IDevice
        {
            private icapContext ic;
            public _device()
            {
                ic = new icapContext();
            }

            public List<Device> GetList(int branchId)
            {
                return (from a in ic.Device
                        join b in ic.BranchDeviceList on a.Id equals b.DeviceId
                        where b.BranchId == branchId && a.DeletedFlag == false
                        orderby a.Id
                        select a).DefaultIfEmpty().ToList();
            }

            public List<string> GetList()
            {
                return ic.Device.Where(x => x.DeletedFlag == false).Select(x => x.Name).ToList();
            }

            public Device Get(string devName)
            {
                return ic.Device.Where(x => x.Name == devName && x.DeletedFlag == false).SingleOrDefault();
            }

            public Device Get(int DeviceId)
            {
                return ic.Device.Where(d => d.DeletedFlag == false && d.Id == DeviceId).SingleOrDefault();
            }

            public int GetID(string devName)
            {
                return ic.Device.Where(x => x.Name == devName && x.DeletedFlag == false).Select(x => x.Id).SingleOrDefault();
            }

            public int Count()
            {
                return ic.Device.Count();
            }
            
            public class DeviceChangedHandler
            {
                string _ip = null;
                string _command = null;
                string _payload = null;
                icapContext _ic = null;
                int _deviceId = 0;
                public DeviceChangedHandler(string ip, string command, string payload, icapContext ic, int deviceId)
                {
                    _ip = ip;
                    _command = command;
                    _payload = payload;
                    _ic = ic;
                    _deviceId = deviceId;
                }

                public void CheckAndSend()
                {
                    var id = (from a in _ic.BranchDeviceList
                              join b in _ic.ThresholdBranchList on a.BranchId equals b.BranchId
                              where a.DeviceId == _deviceId
                              select b.Id).FirstOrDefault();

                    if (id > 0)
                    {
                        var notify = new NotifyCoreService(_ip, _command, _payload);
                        notify.Send();
                    }
                }

            }

            public void Create(Device dev)
            {
                ic.Device.Add(dev);
                ic.SaveChanges();
                int deviceIdLast = ic.Device.OrderBy(d => d.Id).Select(d => d.Id).LastOrDefault();
                ic.BranchDeviceList
                    .Add(new BranchDeviceList() { BranchId = 1, DeviceId = deviceIdLast, CreatedDate = DateTime.UtcNow, LastModifiedDate = DateTime.UtcNow });
                ic.SaveChanges();

                var handler = new DeviceChangedHandler("172.30.0.13", "N", "TH/UPDATE", ic, deviceIdLast);
                handler.CheckAndSend();
                //Thread thread = new Thread(new ThreadStart(handler.CheckAndSend));
                //thread.Start();
            }

            public MapItemTemplate[] GetLocation()
            {
                return
                    (from a in ic.Device
                     join b in ic.Employee on a.OwnerId equals b.Id
                     where a.DeletedFlag == false
                     select new MapItemTemplate()
                     {
                         id = a.Id,
                         name = a.Name,
                         owner = (b.LastName == null || b.LastName.Length == 0) ? ((b.FirstName == null || b.FirstName.Length == 0) ? "" : b.FirstName) : ((b.FirstName == null || b.FirstName.Length == 0) ? b.LastName : b.LastName + " " + b.FirstName),
                         alias = a.Alias
                     }).ToArray();
            }

            public MapItemTemplate GetLocation(string devName)
            {
                return
                    (from a in ic.Device
                     join b in ic.Employee on a.OwnerId equals b.Id
                     where a.Name == devName && a.DeletedFlag == false && b.DeletedFlag == false
                     select new MapItemTemplate()
                     {
                         id = a.Id,
                         name = a.Name,
                         owner = (b.LastName == null || b.LastName.Length == 0) ? ((b.FirstName == null || b.FirstName.Length == 0) ? "" : b.FirstName) : ((b.FirstName == null || b.FirstName.Length == 0) ? b.LastName : b.LastName + " " + b.FirstName),
                         alias = a.Alias
                     }).SingleOrDefault();
            }

            public string GetOwner(string devName)
            {    
                return (from a in ic.Device
                        join b in ic.Employee on a.OwnerId equals b.Id
                        where a.Name == devName && a.DeletedFlag == false && b.DeletedFlag == false
                        select (b.LastName == null || b.LastName.Length == 0) ? ((b.FirstName == null || b.FirstName.Length == 0) ? "" : b.FirstName) : ((b.FirstName == null || b.FirstName.Length == 0) ? b.LastName : b.LastName + " " + b.FirstName)).SingleOrDefault();
            }

            public DeviceProfileTemplate GetDeviceProfile(string devName)
            {
                return
                    (
                        from a in ic.Device
                        join b in ic.Employee on a.OwnerId equals b.Id
                        where a.Name == devName && a.DeletedFlag == false && b.DeletedFlag == false
                        select new DeviceProfileTemplate()
                        {
                            Id = a.Id,
                            DevName = a.Name,
                            Alias = a.Alias,
                            PhotoURL = a.PhotoUrl,
                            OwnerName = (b.LastName == null || b.LastName.Length == 0) ? ((b.FirstName == null || b.FirstName.Length == 0) ? "" : b.FirstName) : ((b.FirstName == null || b.FirstName.Length == 0) ? b.LastName : b.LastName + " " + b.FirstName),
                        }
                    ).SingleOrDefault();
            }

            public bool? UpdateDeviceProfile(DeviceProfileTemplate devProfile)
            {
                try
                {
                    Device dev = ic.Device.Where(x => x.Name == devProfile.DevName && x.DeletedFlag == false).SingleOrDefault();
                    if (dev == null)
                    {
                        return null;
                    }
                    dev.Alias = devProfile.Alias;
                    dev.Longitude = (float)devProfile.Longitude;
                    dev.Latitude = (float)devProfile.Latitude;
                    dev.PhotoUrl = devProfile.PhotoURL;
                    dev.LastModifiedDate = DateTime.UtcNow;
                    ic.Device.Update(dev);
                    ic.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            public bool Delete(string devName)
            {
                try
                {
                    Devicecertificate devcert = (from a in ic.Device join b in ic.Devicecertificate on a.Id equals b.DeviceId where a.Name == devName && a.DeletedFlag == false select b).SingleOrDefault();
                    Device dev = ic.Device.Where(x => x.Name == devName && x.DeletedFlag == false).SingleOrDefault();
                    if (dev == null && devcert == null)
                    {
                        return false;
                    }
                    devcert.DeletedFlag = true;
                    ic.Devicecertificate.Update(devcert);
                    dev.DeletedFlag = true;
                    ic.Device.Update(dev);
                    ic.Database.ExecuteSqlCommand($"DELETE FROM branchdevicelist where DeviceId={dev.Id};");
                    ic.SaveChanges();

                    var handler = new DeviceChangedHandler("172.30.0.13", "N", "TH/UPDATE", ic, dev.Id);

                    Thread thread = new Thread(new ThreadStart(handler.CheckAndSend));
                    thread.Start();
                }
                catch (Exception)
                {
                    throw new Exception("Delete the device Failed! Please refresh the web page and try again.");
                }
                return true;
            }

            public Dictionary<String, String> GetOwner_All()
            {
                Dictionary<string, string> ret;
                ret =
                    (from a in ic.Device
                     join b in ic.Employee on a.OwnerId equals b.Id
                     select new { key = a.Name, value = (b.LastName == null || b.LastName.Length == 0) ? ((b.FirstName == null || b.FirstName.Length == 0) ? "" : b.FirstName) : ((b.FirstName == null || b.FirstName.Length == 0) ? b.LastName : b.LastName + " " + b.FirstName), }
                     ).ToDictionary(d => d.key, d => d.value);

                return ret;
            }
            public Dictionary<String, String> GetAlias_All()
            {
                Dictionary<string, string> ret;
                ret =
                    (from a in ic.Device
                     select new { key = a.Name, value = a.Alias }
                     ).ToDictionary(d => d.key, d => d.value);

                return ret;
            }
            public SelectOptionTemplate[] GetDeviceList()
            {
                var ret = (from a in ic.Device
                           where a.DeletedFlag == false
                           orderby a.Alias
                           select new SelectOptionTemplate()
                           {
                               Id = a.Id,
                               Name = a.Name,
                               Alias = a.Alias
                           }).ToArray();
                if (ret.Length == 0)
                    return null;
                return ret;
            }

            public string GetAlias(string devName)
            {
                return (from d in ic.Device
                        where d.Name == devName && d.DeletedFlag == false
                        select d.Alias).SingleOrDefault();
            }

            public DeviceProfileTemplate GetAliasOwner(string devName)
            {
                return (from d in ic.Device
                        join e in ic.Employee on d.OwnerId equals e.Id
                        where d.Name == devName && d.DeletedFlag == false && e.DeletedFlag == false
                        select new DeviceProfileTemplate()
                        {
                            Alias = d.Alias,
                            OwnerName = (e.LastName == null || e.LastName.Length == 0) ? ((e.FirstName == null || e.FirstName.Length == 0) ? "" : e.FirstName) : ((e.FirstName == null || e.FirstName.Length == 0) ? e.LastName : e.LastName + " " + e.FirstName),
                        }).SingleOrDefault();
            }

            public string Screenshot(string devName)
            {
                NotifyCoreService notifyCoreService = new NotifyCoreService();

                notifyCoreService.Connect("172.30.0.11");
                notifyCoreService.Send("INNO", "D", JsonConvert.SerializeObject(new
                {
                    Cmd = (int)Action.Screenshot,
                    DevName = devName
                }));
                string jsonStr = notifyCoreService.ReceiveMessage();
                notifyCoreService.Close();
                return jsonStr;
            }

            public Device[] GetDeviceList(int branchId)
            {
                return (from a in ic.Device
                        join b in ic.BranchDeviceList on a.Id equals b.DeviceId
                        where b.BranchId == branchId && a.DeletedFlag == false
                        orderby a.Id
                        select new Device
                        {
                            Name = a.Name,
                            Alias = a.Alias,
                        }).ToArray();
            }

            public bool UpdateAlias(string devName, string alias)
            {
                Device dev = ic.Device.Where(d => d.Name == devName).SingleOrDefault();

                if (dev == null) return false;

                dev.Alias = alias;
                dev.LastModifiedDate = DateTime.UtcNow;
                ic.Device.Update(dev);
                ic.SaveChanges();
                return true;
            }
        }
        public class _devicecertificate : IDevicecertificate
        {
            public Devicecertificate Get(string Thumbprint)
            {
                icapContext ic = new icapContext();
                try
                {
                    return ic.Devicecertificate.Where(x => x.Thumbprint == Thumbprint && x.DeletedFlag == false).SingleOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public void Update(Devicecertificate cert)
            {
                icapContext ic = new icapContext();
                ic.Devicecertificate.Update(cert);
                ic.SaveChanges();
            }

            public void Create(Devicecertificate cert)
            {
                icapContext ic = new icapContext();
                ic.Devicecertificate.Add(cert);
                ic.SaveChanges();
            }
        }

        public class _employee : Image, IEmployee
        {
            private icapContext ic;
            const int GUEST = 1;
            const int ADMIN = 2;
            IRedisCacheDispatcher rcd;
            public _employee()
            {
                ic = new icapContext();
                rcd = new RedisCacheDispatcher();
            }

            public string GetToken(string username)
            {
                var all = rcd.GetAll(0);
                string token = null;
                try
                {
                    token = all.Where(item => item.Value == username).Select(item => item.Key).SingleOrDefault();
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                    foreach (var item in all)
                    {
                        if (item.Value == username)
                        {
                            rcd.KeyDelete(0, item.Key);
                        }
                    }
                }

                if (token == null)
                {
                    token = Convert.ToBase64String(Encoding.UTF8.GetBytes(CommonFunctions.CreateRandomPassword(16)));
                    rcd.SetCache(0, token, username);
                }
                return token;
            }
            public bool Create(EmployeeProfileTemplate e)
            {
                try
                {
                    ic.Employee.Add(new Employee()
                    {
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        CompanyId = 1,
                        Email = e.Email,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        LoginName = e.LoginName,
                        PermissionId = (e.AdminFlag == true) ? ADMIN : GUEST,
                        Password = e.PWD,
                        EmployeeNumber = e.EmployeeNumber,
                        MapId = 5
                    });
                    ic.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            public EmployeeProfileTemplate Get(string LoginName)
            {
                return (from a in ic.Employee
                        where a.LoginName == LoginName
                        select new EmployeeProfileTemplate
                        {
                            LoginName = a.LoginName,
                            Email = a.Email,
                            EmployeeNumber = a.EmployeeNumber,
                            FirstName = a.FirstName,
                            LastName = a.LastName,
                            AdminFlag = (a.PermissionId == ADMIN) ? true : false
                        }).SingleOrDefault();
            }

            public class EmployeeUpdatedHandler
            {
                string _ip = null;
                string _command = null;
                string _payload = null;
                icapContext _ic = null;
                int _employeeId = 0;
                public string _email = null;
                const int EMAIL_ACTION = 1;

                public EmployeeUpdatedHandler(string ip, string command, string payload, icapContext ic, int employeeId)
                {
                    _ip = ip;
                    _command = command;
                    _payload = payload;
                    _ic = ic;
                    _employeeId = employeeId;
                }

                public void CheckAndSend()
                {

                    if (_email != null)
                    {
                        var externalRecipient = (from a in _ic.ExternalRecipient
                                                 where a.Email == _email
                                                 select a).SingleOrDefault();

                        if (externalRecipient != null)
                        {
                            var thresholdIdList = (from a in _ic.ThresholdExternalRecipientList
                                                   where a.ExternalRecipientId == externalRecipient.Id
                                                   select a.ThresholdId).ToArray();

                            foreach (int id in thresholdIdList)
                            {
                                _ic.ThresholdEmployeeList.Add(new ThresholdEmployeeList()
                                {
                                    ThresholdId = id,
                                    EmployeeId = _employeeId
                                });
                            }
                            _ic.ExternalRecipient.Remove(externalRecipient);
                        }
                    }

                    var thresholdId = (from a in _ic.Threshold
                                       join b in _ic.ThresholdEmployeeList on a.Id equals b.ThresholdId
                                       where Convert.ToBoolean(a.Action & EMAIL_ACTION) || b.EmployeeId == _employeeId
                                       select b.ThresholdId).FirstOrDefault();

                    if (thresholdId > 0)
                    {
                        var notify = new NotifyCoreService(_ip, _command, _payload);
                        notify.Send();
                        return;
                    }
                    else
                    {
                        thresholdId = (from a in _ic.Threshold
                                       join b in _ic.ThresholdPermissionList on a.Id equals b.ThresholdId
                                       join c in _ic.Employee on b.PermissionId equals c.PermissionId
                                       where c.Id == _employeeId && Convert.ToBoolean(a.Action & EMAIL_ACTION)
                                       select b.ThresholdId).FirstOrDefault();

                        if (thresholdId > 0)
                        {
                            var notify = new NotifyCoreService(_ip, _command, _payload);
                            notify.Send();
                            return;
                        }
                    }
                }

            }

            public bool? Update(EmployeeProfileTemplate p)
            {

                Employee e = ic.Employee.Where(x => x.LoginName == p.LoginName).SingleOrDefault();

                if (e == null)
                {
                    return null;
                }

                if (e.Email != p.Email)
                {
                    e.Email = p.Email;
                   
                    var handler = new EmployeeUpdatedHandler("172.30.0.13", "N", "TH/UPDATE", ic, e.Id);
                    handler._email = p.Email;
                    var thread = new Thread(new ThreadStart(handler.CheckAndSend));
                    thread.Start();
                }

                e.EmployeeNumber = p.EmployeeNumber;
                e.FirstName = p.FirstName;
                e.LastName = p.LastName;
                e.PermissionId = (p.AdminFlag == true) ? ADMIN : GUEST;
                if (p.PWD != null)
                {
                    e.Password = p.PWD;
                }
                e.LastModifiedDate = DateTime.UtcNow;
                ic.Employee.Update(e);
                try
                {
                    ic.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    return false;
                }
                return true;
            }

            public void SetCommonMap(string loginName, int mapId)
            {
                Employee e = ic.Employee.Where(x => x.LoginName == loginName && x.DeletedFlag == false).SingleOrDefault();

                if (e != null)
                {
                    e.MapId = mapId;
                    try
                    {
                        ic.Employee.Update(e);
                        ic.SaveChanges();
                    }
                    catch (DbUpdateException exc)
                    {
                        throw exc;
                    }
                }
            }

            private string GetUserGroupTooltipContent(string lastName, string firstName, string email)
            {
                var name = (lastName == null || lastName.Length == 0) ? ((firstName == null || firstName.Length == 0) ? "" : firstName) : ((firstName == null || firstName.Length == 0) ? lastName : lastName + " " + firstName);

                return (name == "") ? email : name + $" &lt{email}&gt";
            }

            public EmailSearchTemplate[] FindAllEmail(string searchString)
            {
                List<EmailSearchTemplate> internalRecipientList = (from a in ic.Employee
                                                                   where a.DeletedFlag == false
                                                                   && a.Email != null
                                                                   && ((a.Email.StartsWith(searchString, StringComparison.OrdinalIgnoreCase))
                                                                   || (a.LastName ?? "" + a.FirstName ?? "").IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                                                                   select new EmailSearchTemplate()
                                                                   {
                                                                       label = GetEmailLabel(a.FirstName, a.LastName, a.Email),
                                                                       employeeId = a.Id
                                                                   }).ToList();

                List<EmailSearchTemplate> permissionList = (from a in ic.Permission
                                                            where a.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
                                                            select new EmailSearchTemplate()
                                                            {
                                                                label = a.Name,
                                                                permissionId = a.Id
                                                            }).ToList();

                List<EmailSearchTemplate> externalRecipientList = (from a in ic.ExternalRecipient
                                                                   where a.Email.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)
                                                                   select new EmailSearchTemplate()
                                                                   {
                                                                       label = a.Email,
                                                                       externalRecipientId = a.Id
                                                                   }).ToList();
                

                return internalRecipientList.Concat(permissionList).Concat(externalRecipientList).ToArray();
            }

            public string[] GetEmail(int permissionId)
            {
                return (from a in ic.Employee
                        where a.PermissionId == permissionId && a.Email != null
                        select GetEmailLabel(a.FirstName, a.LastName, a.Email)).ToArray();
            }

            public string[] GetEmailForTooltip(int permissionId)
            {
                return (from a in ic.Employee
                        where a.PermissionId == permissionId && a.Email != null
                        select GetUserGroupTooltipContent(a.LastName, a.FirstName, a.Email)
                        ).ToArray();
            }

            public int GetUserCommonMap(string loginName)
            {
                var ret = (from a in ic.Employee
                           where a.DeletedFlag == false && a.LoginName == loginName
                           select a.MapId).SingleOrDefault();

                return ret;
            }
            public bool? Delete(string loginName)
            {
                Employee e = ic.Employee.Where(x => x.LoginName == loginName).SingleOrDefault();
                if (e == null)
                {
                    return null;
                }
                try
                {
                    //Branchroles[] br = (from a in ic.Branchroles
                    //                    join b in ic.Employee on a.EmployeeId equals b.Id
                    //                    where b.LoginName == loginName
                    //                    select a).ToArray();
                    //br = br.Select(b =>
                    //{
                    //    b.EmployeeId = 1;
                    //    return b;
                    //}).ToArray();
                    //ic.Branchroles.UpdateRange(br);

                    Device[] dev = (from a in ic.Device
                                    join b in ic.Employee on a.OwnerId equals b.Id
                                    where b.LoginName == loginName && a.DeletedFlag == false
                                    select a).ToArray();
                    dev = dev.Select(d =>
                    {
                        d.OwnerId = 1;
                        return d;
                    }).ToArray();
                    ic.Device.UpdateRange(dev);
                    //ic.Employee.Attach(e);
                    ic.Employee.Remove(e);
                    ic.SaveChanges();

                    var handler = new EmployeeUpdatedHandler("172.30.0.13", "N", "TH/UPDATE", ic, e.Id);
                    var thread = new Thread(new ThreadStart(handler.CheckAndSend));
                    thread.Start();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            public bool CheckExists(string LoginName)
            {
                Employee p = ic.Employee.Where(x => x.LoginName == LoginName).SingleOrDefault();
                return (p != null);
            }

            public bool CheckPWD(string Username, string Password)
            {
                Employee p = ic.Employee.Where(x => x.LoginName == Username).SingleOrDefault();
                if (p != null)
                {
                    return string.Equals(p.Password,
                            Convert.ToBase64String(Encoding.UTF8.GetBytes(Password)));
                }
                return false;
            }

            public bool CheckAdmin(string loginName)
            {
                return (from a in ic.Employee where a.LoginName == loginName select (a.PermissionId == ADMIN) ? true : false ).SingleOrDefault();
            }

            public string[] GetList()
            {
                return (from a in ic.Employee select a.LoginName).ToArray();
            }

            public SelectOptionTemplate[] GetName()
            {
                return (from a in ic.Employee
                        where a.DeletedFlag == false && a.Email != null
                        select new SelectOptionTemplate()
                        {
                            Id = a.Id,
                            Name = (a.LastName == null || a.LastName.Length == 0) ? ((a.FirstName == null || a.FirstName.Length == 0) ? "" : a.FirstName) : ((a.FirstName == null || a.FirstName.Length == 0) ? a.LastName : a.LastName + " " + a.FirstName),
                        }).ToArray();
            }

           public object GetEmployeeInfo(string email)
           {
                return (from a in ic.Employee
                        where a.DeletedFlag == false && a.Email == email
                        select new
                        {
                            a.Id,
                            //Name = (a.LastName ?? "") + ((a.FirstName == null || a.FirstName.Length == 0) ? "" : (" " + a.FirstName)),
                            Name = (a.LastName == null || a.LastName.Length == 0) ? ((a.FirstName == null || a.FirstName.Length == 0) ? "" : a.FirstName) : ((a.FirstName == null || a.FirstName.Length == 0) ? a.LastName : a.LastName + " " + a.FirstName)
                        }).SingleOrDefault();
           }
        }

        public class _licenselist : ILicenselist
        {
            private icapContext ic;
            private KeyChecker ky;
            public _licenselist()
            {
                ic = new icapContext();
                ky = new KeyChecker();
            }

            public bool OverLimitation()
            {
                icapContext ic = new icapContext();
                KeyChecker ky = new KeyChecker();
                int deviceCount = ic.Device.Where(d => d.DeletedFlag == false).Count();

                int TotalDeviceCount = ky.GetAvailableDeviceCount();
#if true
                return (deviceCount >= TotalDeviceCount);
#else
                return (deviceCount >= 5);
#endif
            }

            public string[] GetDeviceListToSetOffline()
            {
                int AvailableCount = ky.GetAvailableDeviceCount();
                Device[] devices = ic.Device.Where(d => d.DeletedFlag == false).OrderBy(d => d.Id).Skip(AvailableCount).ToArray();
                List<string> DeviceList = new List<string>();

                if (devices.Length != 0)
                {
                    foreach (Device element in devices)
                    {
                        Devicecertificate devcert = (from a in ic.Device
                                                     join b in ic.Devicecertificate on a.Id equals b.DeviceId
                                                     where b.DeviceId == element.Id && a.DeletedFlag == false
                                                     select b).SingleOrDefault();
                        devcert.DeletedFlag = true;
                        ic.Devicecertificate.Update(devcert);

                        element.DeletedFlag = true;
                        ic.Device.Update(element);
                        ic.Database.ExecuteSqlCommand($"DELETE FROM branchdevicelist where DeviceId={element.Id};");
                        ic.SaveChanges();
                        DeviceList.Add(element.Name);
                    }
                    return DeviceList.ToArray();
                }
                return null;
            }
            public void Create(LicenseList lic)
            {
                icapContext ic = new icapContext();
                ic.LicenseList.Add(lic);
                ic.SaveChanges();
            }

            public int DeviceCount()
            {
                icapContext ic = new icapContext();
                List<LicenseList> p = ic.LicenseList.Where(x => x.Id > 0).ToList();

                int TotalDeviceCount = 0;

                foreach (LicenseList l in p)
                {
                    TotalDeviceCount += l.DeviceCount;
                }

                return TotalDeviceCount;
            }
        }

        public class _companydashboardlist
        {
            public KeyValuePair<int, string>[] GetList(int CompanyId)
            {
                icapContext ic = new icapContext();
                return (from a in ic.Companydashboard
                        where a.CompanyId == CompanyId && a.DeletedFlag == false
                        select new KeyValuePair<int, string>(a.Id, a.Name)).ToArray();
            }
        }

        public class _companydashboardelement
        {
            public PanelItemTemplate[] Get(int DashboardId)
            {
                icapContext ic = new icapContext();
                return (from a in ic.Companydashboardelement
                        join b in ic.Widget on a.WidgetId equals b.Id
                        join c in ic.Data on b.DataId equals c.Id
                        join d in ic.Chart on b.ChartId equals d.Id
                        where a.DashboardId == DashboardId && a.DeletedFlag == false && b.DeletedFlag == false
                        orderby a.IteratorIndex
                        select new PanelItemTemplate()
                        {
                            WidgetId = b.Id,
                            WidgetName = b.Name,
                            WidgetType = d.Name,
                            WidgetWidth = b.Width,
                            DataName = c.Name,
                            DataCount = b.DataCount,
                            DataLocation = c.Location,
                            SettingStr = b.SettingStr
                        }).ToArray();
            }
        }

        public class _Data
        {
            public Data Get(int id)
            {
                icapContext ic = new icapContext();
                return ic.Data.Where(x => x.Id == id).SingleOrDefault();
            }
        }

        public class _widget : IWidgetRepository
        {
            private icapContext ic;

            public _widget()
            {
                ic = new icapContext();
            }
            public bool CheckDataExists(int? Id)
            {
                Data p = ic.Data.Where(x => x.Id == Id).SingleOrDefault();
                return (p != null);
            }

            private void SettingStrProcessing(ref string SettingStr, int? dataId)
            {
                SettingItemTemplate sit = JsonConvert.DeserializeObject<SettingItemTemplate>(SettingStr);

                switch (sit.Func)
                {
                    //case 0:
                    //    var dataName = (from a in ic.Data
                    //                    where a.Id == dataId
                    //                    select a.Name).SingleOrDefault();

                    //    DisableFormat disableSettingStr = new DisableFormat()
                    //    {
                    //        Label = new string[] { dataName },
                    //        Func = 0
                    //    };
                    //    SettingStr = JsonConvert.SerializeObject(disableSettingStr);
                    //    break;
                    case 2:
                        Array.Sort(sit.Divider.Percentage);
                        Array.Reverse(sit.Divider.Percentage);
                        if (sit.Divider.Percentage.Length == 1)
                        {
                            PercentageFormat percentageSettingStr = new PercentageFormat()
                            {
                                Label = new string[] { $"≥ {sit.Divider.Percentage[0]} %", $"< {sit.Divider.Percentage[0]} %" },
                                Func = 2,
                                Divider = new PercentageFormat.SettingDividerTemplate()
                                {
                                    Percentage = sit.Divider.Percentage,
                                    DenominatorId = sit.Divider.DenominatorId,
                                    //DataName = sit.Divider.DataName
                                }
                            };
                            SettingStr = JsonConvert.SerializeObject(percentageSettingStr);
                            break;
                        }
                        else
                        {
                            sit.Label = new string[sit.Divider.Percentage.Length + 1];
                            sit.Label[0] = $"≥ {sit.Divider.Percentage[0]} %";
                            for (int i = 1; i <= sit.Divider.Percentage.Length; i++)
                            {
                                if (i != sit.Divider.Percentage.Length)
                                {
                                    sit.Label[i] = $"{sit.Divider.Percentage[i]} % - {sit.Divider.Percentage[i - 1]} %";
                                }
                                else
                                {
                                    sit.Label[i] = $"< {sit.Divider.Percentage[i - 1]} %";
                                }
                            }

                            PercentageFormat percentageSettingStr = new PercentageFormat()
                            {
                                Label = sit.Label,
                                Func = 2,
                                Divider = new PercentageFormat.SettingDividerTemplate()
                                {
                                    Percentage = sit.Divider.Percentage,
                                    DenominatorId = sit.Divider.DenominatorId,
                                    
                                    //DataName = sit.Divider.DataName
                                }
                            };
                            SettingStr = JsonConvert.SerializeObject(percentageSettingStr);
                            break;
                        }
                    case 3:
                        BooleanFormat booleanSettingStr = new BooleanFormat()
                        {
                            Label = sit.Label,
                            Func = 3,
                            Divider = new BooleanFormat.SettingDividerTemplate()
                            {
                                Boolean = new bool[] { true, false }
                            }
                        };
                        SettingStr = JsonConvert.SerializeObject(booleanSettingStr);
                        break;
                    case 4:
                        Array.Sort(sit.Divider.Number);
                        Array.Reverse(sit.Divider.Number);
                        var unit = ic.Data.Where(d => d.Id == dataId).Select(d => d.Unit).SingleOrDefault();

                        if (sit.Divider.Number.Length == 1)
                        {
                            NumbericalFormat numbericalSettingStr = new NumbericalFormat()
                            {
                                Label = new string[] { $"≥ {sit.Divider.Number[0]} " + unit, $"< {sit.Divider.Number[0]} " + unit },
                                Func = 4,
                                Divider = new NumbericalFormat.SettingDividerTemplate()
                                {
                                    Number = sit.Divider.Number
                                }
                            };
                            SettingStr = JsonConvert.SerializeObject(numbericalSettingStr);
                            break;
                        }
                        else
                        {
                            sit.Label = new string[sit.Divider.Number.Length + 1];
                            sit.Label[0] = $"≥ {sit.Divider.Number[0]} " + unit;
                            for (int i = 1; i <= sit.Divider.Number.Length; i++)
                            {
                                if (i != sit.Divider.Number.Length)
                                {
                                    sit.Label[i] = $"{sit.Divider.Number[i]} " + unit + " - " + $"{sit.Divider.Number[i - 1]} " + unit;
                                }
                                else
                                {
                                    sit.Label[i] = $"< {sit.Divider.Number[i - 1]} " + unit;
                                }
                            }

                            NumbericalFormat numbericalSettingStr = new NumbericalFormat()
                            {
                                Label = sit.Label,
                                Func = 4,
                                Divider = new NumbericalFormat.SettingDividerTemplate()
                                {
                                    Number = sit.Divider.Number
                                }
                            };
                            SettingStr = JsonConvert.SerializeObject(numbericalSettingStr);
                            break;
                        }
                }
            }

            private bool BranchIdExist(int[] branchIdList_Noduplicate)
            {
                int[] branchIdList_AdminDB = ic.Branch.Where(b => b.DeletedFlag == false).Select(b => b.Id).ToArray();

                foreach (int branchId in branchIdList_Noduplicate)
                {
                    if (!branchIdList_AdminDB.Contains(branchId))
                        return false;
                }
                return true;
            }

            public void Create(string widgetName)
            {
                try
                {
                    ic.Widget.Add(new Widget()
                    {
                        Name = widgetName,
                        DeletedFlag = false,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                    });
                    ic.SaveChanges();
                }
                catch (DbUpdateException exc)
                {
                    throw exc;
                }
            }

            public void Create(WidgetTemplate w)
            {
                if (w.BranchIdList.Length == 0)
                    throw new Exception();

                string SettingStr = w.SettingStr;
                int[] branchIdList_Noduplicate = w.BranchIdList.Distinct().ToArray();

                if (!BranchIdExist(branchIdList_Noduplicate))
                {
                    throw (new Exception("The BRANCH could NOT be found! Please check the field or refresh the web page."));
                }

                try
                {
                    SettingStrProcessing(ref SettingStr, w.DataId);
                }
                catch (Exception)
                {
                    throw (new Exception("Data Processing Error! Please fill in the form again."));
                }

                try
                {
                    if (w.DataCount != 1)
                    {
                        string chartName = (from a in ic.Chart
                                            where a.Id == w.ChartId
                                            select a.Name).SingleOrDefault();
                        if (chartName != "line")
                            w.DataCount = 1;
                        if (chartName == "line" && w.DataCount == 0)
                            w.DataCount = 1;
                    }

                    ic.Widget.Add(new Widget()
                    {
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        Name = w.Name,
                        DataId = w.DataId,
                        DataCount = w.DataCount,
                        ChartId = w.ChartId,
                        Width = w.Width,
                        SettingStr = SettingStr,
                        DeletedFlag = false,
                    });

                    ic.SaveChanges();

                    int LastWidgetId = ic.Widget.OrderBy(wid => wid.Id).Select(wid => wid.Id).LastOrDefault();

                    try
                    {
                        foreach (int branchId in branchIdList_Noduplicate)
                        {
                            ic.WidgetBranchList.Add(new WidgetBranchList()
                            {
                                CreatedDate = DateTime.UtcNow,
                                LastModifiedDate = DateTime.UtcNow,
                                WidgetId = LastWidgetId,
                                BranchId = branchId
                            });
                        }
                        ic.SaveChanges();
                    }
                    catch (DbUpdateException exc)
                    {
                        ic.Database.ExecuteSqlCommand($"DELETE FROM widget where Id={LastWidgetId};");
                        throw exc;
                    }
                }
                catch (DbUpdateException exc)
                {
                    throw exc;
                }
                catch (Exception)
                {
                    throw new Exception("Create the card failed! Please check all fields.");
                }
            }

            private string[] getLabel(string settingStr, int? dataId, List<ThresholdSettingTemplate> thresholdSetting)
            {
                SettingItemTemplate sit = JsonConvert.DeserializeObject<SettingItemTemplate>(settingStr);
                string unit;
                switch (sit.Func)
                {
                    case 2:
                        Array.Sort(sit.Divider.Percentage);
                        Array.Reverse(sit.Divider.Percentage);
                        if (sit.Divider.Percentage.Length == 1)
                        {
                            return new string[] { $"≥ {sit.Divider.Percentage[0]} %", $"< {sit.Divider.Percentage[0]} %" };
                        }
                        else
                        {
                            var label = new string[sit.Divider.Percentage.Length + 1];

                            label[0] = $"≥ {sit.Divider.Percentage[0]} %";
                            for (int i = 1; i <= sit.Divider.Percentage.Length; i++)
                            {
                                if (i != sit.Divider.Percentage.Length)
                                {
                                    label[i] = $"{sit.Divider.Percentage[i]} % - {sit.Divider.Percentage[i - 1]} %";
                                }
                                else
                                {
                                    label[i] = $"< {sit.Divider.Percentage[i - 1]} %";
                                }
                            }
                            return label;
                        }
                    case 3:
                        return sit.Label;
                    case 4:
                        Array.Sort(sit.Divider.Number);
                        Array.Reverse(sit.Divider.Number);
                        unit = ic.Data.Where(d => d.Id == dataId).Select(d => d.Unit).SingleOrDefault();

                        if (sit.Divider.Number.Length == 1)
                        {
                            return new string[] { $"≥ {sit.Divider.Number[0]} " + unit, $"< {sit.Divider.Number[0]} " + unit };
                        }
                        else
                        {
                            var label = new string[sit.Divider.Number.Length + 1];
                            label[0] = $"≥ {sit.Divider.Number[0]} " + unit;

                            for (int i = 1; i <= sit.Divider.Number.Length; i++)
                            {
                                if (i != sit.Divider.Number.Length)
                                {
                                    label[i] = $"{sit.Divider.Number[i]} " + unit + " - " + $"{sit.Divider.Number[i - 1]} " + unit;
                                }
                                else
                                {
                                    label[i] = $"< {sit.Divider.Number[i - 1]} " + unit;
                                }
                            }
                            return label;
                        }
                }
                return null;
            }

            private class WidgetDetail
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public string ChartName { get; set; }
                public string Width { get; set; }
                public string SettingStr { get; set; }
                public int? DataId { get; set; }
            }

            public WidgetInfo[] GetWidgetInfo()
            {
                WidgetDetail[] allWidget =
                (
                    from a in ic.Widget
                    join b in ic.Chart on a.ChartId equals b.Id
                    orderby a.Width  // follow the width (small to large)
                    where a.DeletedFlag == false && a.ChartId != null
                    select new WidgetDetail
                    {
                        Id = a.Id,
                        Name = a.Name,
                    }
                ).ToArray();

                if (allWidget.Length == 0)
                    return null;

                WidgetInfo[] widgetInfo = new WidgetInfo[allWidget.Length];
                var index = 0;
                foreach (WidgetDetail widget in allWidget)
                {
                    widgetInfo[index] = new WidgetInfo();
                    widgetInfo[index].Id = widget.Id;
                    widgetInfo[index].Name = widget.Name;
                    index++;
                }
                return widgetInfo;
            }

            public WidgetTemplate Get(int Id)
            {
                int[] BranchIdArray = ic.WidgetBranchList
                    .Where(wbl => wbl.WidgetId == Id)
                    .OrderBy(wbl => wbl.BranchId)
                    .Select(wbl => wbl.BranchId).ToArray();
                if (0 == BranchIdArray.Length)
                {
                    BranchIdArray = null;
                }
                Widget widget = ic.Widget.Where(w => w.Id == Id && w.DeletedFlag == false).SingleOrDefault();

                int? dataGroupId = null;

                if (null != widget.DataId)
                {
                    dataGroupId = ic.Data.Where(d => d.Id == widget.DataId).Select(d => d.GroupId).SingleOrDefault();
                }

                WidgetTemplate widgetTemplate = new WidgetTemplate()
                {
                    WidgetId = widget.Id,
                    Name = widget.Name,
                    DataId = widget.DataId,
                    DataCount = widget.DataCount,
                    ChartId = widget.ChartId,
                    Width = widget.Width,
                    BranchIdList = BranchIdArray,
                    DataGroupId = dataGroupId,
                    ProcessType = null,
                    ThresholdId = widget.ThresholdId
                };

                if (null != widget.SettingStr && "" != widget.SettingStr)
                {
                    SettingItemTemplate sit = JsonConvert.DeserializeObject<SettingItemTemplate>(widget.SettingStr);
                    widgetTemplate.ProcessType = sit.Func;

                    switch (sit.Func)
                    {
                        case 2:
                            widgetTemplate.Percentage = new DataTemplate.Percentage() { Divider = sit.Divider.Percentage, DenominatorId = sit.Divider.DenominatorId };
                            break;
                        case 3:
                            widgetTemplate.Boolean = new DataTemplate.Boolean() { label = sit.Label };
                            break;
                        case 4:
                            widgetTemplate.Numberical = new DataTemplate.Numberical() { Divider = sit.Divider.Number };
                            break;
                        case 5:
                            widgetTemplate.MapCenter = new MapItemTemplate.Pos() { lng = sit.Lng, lat = sit.Lat };
                            break;
                        case 6:
                            widgetTemplate.MapIndex = sit.MapIndex;
                            break;
                    }
                }
                return widgetTemplate;
            }

            public bool Update(WidgetTemplate w)
            {
                const int DEVICE_LOCATION_DATA = 5;
                Widget wg = ic.Widget.Where(x => x.Id == w.WidgetId && x.DeletedFlag == false).SingleOrDefault();

                if (wg == null)
                {
                    return false;
                }
                string settingStr = w.SettingStr;
                try
                {
                    if (w.ThresholdId != null)
                    {
                        settingStr = "{\"Label\":null,\"Func\":1,\"Divider\":null}";
                    } 
                    else if (DEVICE_LOCATION_DATA != w.DataId)
                    {
                        SettingStrProcessing(ref settingStr, w.DataId);
                    } 
                
                }
                catch (Exception)
                {
                    throw (new Exception("Data Processing Error!"));
                }
                int[] branchIdList_Noduplicate = w.BranchIdList.Distinct().ToArray();

                try
                {
                    wg.ChartId = w.ChartId;
                    wg.Name = w.Name;
                    wg.DataId = w.DataId;
                    wg.DataCount = 1;
                    wg.ChartId = w.ChartId;
                    wg.Width = w.Width;
                    wg.ThresholdId = w.ThresholdId;
                    wg.SettingStr = settingStr;
                    wg.LastModifiedDate = DateTime.UtcNow;

                    ic.Database.ExecuteSqlCommand($"DELETE FROM widgetbranchlist where WidgetId={wg.Id};");
                    foreach (int b in w.BranchIdList)
                    {
                        ic.WidgetBranchList.Add(new WidgetBranchList()
                        {
                            CreatedDate = DateTime.UtcNow,
                            WidgetId = w.WidgetId,
                            DeletedFlag = false,
                            LastModifiedDate = DateTime.UtcNow,
                            BranchId = b
                        });
                    }     
                    ic.Widget.Update(wg);
                    ic.SaveChanges();
                    return true;
                }
                catch (DbUpdateException exc)
                {
                    throw exc;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            public void Delete(int Id)
            {
                Widget w = ic.Widget.Where(x => x.Id == Id && x.DeletedFlag == false).SingleOrDefault();
                try
                {
                    w.DeletedFlag = true;
                    w.LastModifiedDate = DateTime.UtcNow;
                    ic.Widget.Update(w);
                    ic.Database.ExecuteSqlCommand($"DELETE FROM companydashboardelement where WidgetId={Id};");
                    ic.Database.ExecuteSqlCommand($"DELETE FROM widgetbranchlist where WidgetId={Id};");
                    ic.SaveChanges();
                }
                catch (Exception)
                {
                    if (w == null)
                        throw new Exception("The card could NOT be found!");

                    throw new Exception("Delete the card Failed! Please refresh the web page and try again.");
                }
            }

            public SelectOptionTemplate[] GetDataGroupList()
            {
                return (from a in ic.DataGroup
                        select new SelectOptionTemplate
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).DefaultIfEmpty().ToArray();
            }
            //public DataSelect[] GetDataSelect()
            //{
            //    DataOption[] dataList =
            //    (
            //        from a in ic.Data
            //        join b in ic.DataGroup on a.GroupId equals b.Id
            //        where a.Numberical && a.Name != "Device latitude" && a.Name != "Device longitude"
            //        select new DataOption
            //        {
            //            DataId = a.Id,
            //            DataName = a.Name,
            //            DataUnit = a.Unit,
            //            GroupId = a.GroupId,
            //            GroupName = b.Name
            //        }
            //    ).DefaultIfEmpty().ToArray();

            //    List<DataSelect> dataSelect = new List<DataSelect>();
            //    List<SelectOptionTemplate> dataOption = new List<SelectOptionTemplate>();
            //    var dataElement = new DataSelect() { GroupOption = new SelectOptionTemplate() };
            //    var option = new SelectOptionTemplate() { Id = 5, Name = "Device Location" };
            //    dataOption.Add(option);
            //    dataElement.GroupOption.Id = dataList[0].GroupId;
            //    dataElement.GroupOption.Name = dataList[0].GroupName;

            //    foreach (DataOption element in dataList)
            //    {
            //        if (element.GroupId != dataElement.GroupOption.Id)
            //        {
            //            dataElement.DataOption = dataOption.ToArray();
            //            dataSelect.Add(dataElement);
            //            dataElement = new DataSelect() { GroupOption = new SelectOptionTemplate() };
            //            dataElement.GroupOption.Id = element.GroupId;
            //            dataElement.GroupOption.Name = element.GroupName;
            //            option = new SelectOptionTemplate();
            //            option.Id = element.DataId;
            //            option.Name = element.DataName;
            //            option.Unit = element.DataUnit;
            //            dataOption = new List<SelectOptionTemplate>();
            //            dataOption.Add(option);
            //        }
            //        else
            //        {
            //            option = new SelectOptionTemplate();
            //            option.Id = element.DataId;
            //            option.Name = element.DataName;
            //            option.Unit = element.DataUnit;
            //            dataOption.Add(option);
            //        }
            //    }
            //    dataElement.DataOption = dataOption.ToArray();
            //    dataSelect.Add(dataElement);
            //    return dataSelect.ToArray();
            //}
            public SelectOptionTemplate[] GetBranchSelect()
            {
                return (from a in ic.Branch
                        where a.DeletedFlag == false
                        select new SelectOptionTemplate()
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).DefaultIfEmpty().ToArray();
            }

            //public ChartWidthSelect[] GetChartWidthSelect(int? dataId)
            //{
            //    ChartWidthSelect[] chartWidthSelect =
            //        (
            //            from a in ic.Chart
            //            join b in ic.DataChartList on a.Id equals b.ChartId
            //            where b.DataId == dataId
            //            orderby b.ChartId
            //            select new ChartWidthSelect
            //            {
            //                Id = a.Id,
            //                Name = a.Name,
            //                ChartSize = new ChartSizeTemplate()
            //                {
            //                    Small = Convert.ToBoolean(Convert.ToInt32("001", 2) & a.SizeFlag),
            //                    Medium = Convert.ToBoolean(Convert.ToInt32("010", 2) & a.SizeFlag),
            //                    Large = Convert.ToBoolean(Convert.ToInt32("100", 2) & a.SizeFlag),
            //                }
            //            }).ToArray();
            //    if (chartWidthSelect.Length == 0)
            //        return null;

            //    return chartWidthSelect;

            //}

            public NumbericalDataOption GetNumbericalDataList()
            {
                DataOption[] dataOption =
                    (
                        from a in ic.Data
                        where a.Numberical && a.Name != "Device latitude" && a.Name != "Device longitude"
                        select new DataOption
                        {
                            DataId = a.Id,
                            DataName = a.Name,
                            GroupId = a.GroupId
                        }
                    ).DefaultIfEmpty().ToArray();
                List<DataOption> system = new List<DataOption>();
                List<DataOption> cpu = new List<DataOption>();
                List<DataOption> memory = new List<DataOption>();
                List<DataOption> storage = new List<DataOption>();
                List<DataOption> external = new List<DataOption>();
                system.Add(new DataOption() { DataId = 5, DataName = "Device Location" });
                foreach (DataOption element in dataOption)
                {
                    switch (element.GroupId)
                    {
                        case 1:
                            system.Add(new DataOption() { DataId = element.DataId, DataName = element.DataName });
                            break;
                        case 2:
                            cpu.Add(new DataOption() { DataId = element.DataId, DataName = element.DataName });
                            break;
                        case 4:
                            memory.Add(new DataOption() { DataId = element.DataId, DataName = element.DataName });
                            break;
                        case 5:
                            storage.Add(new DataOption() { DataId = element.DataId, DataName = element.DataName });
                            break;
                        case 7:
                            external.Add(new DataOption() { DataId = element.DataId, DataName = element.DataName });
                            break;
                    }
                }

                return new NumbericalDataOption()
                {
                    System = system.ToArray(),
                    CPU = cpu.ToArray(),
                    Memory = memory.ToArray(),
                    Storage = storage.ToArray(),
                    External = external.ToArray()
                };
            }

            public SelectOptionTemplate[] GetChartSelect(int? DataId)
            {
                var ret = (from a in ic.Chart
                           join b in ic.DataChartList on a.Id equals b.ChartId
                           where b.DataId == DataId
                           orderby b.ChartId
                           select new SelectOptionTemplate
                           {
                               Id = a.Id,
                               Name = a.Name
                           }).ToArray();
                if (ret.Length == 0)
                    return new SelectOptionTemplate[] { null };
                return ret;
            }

            public object[] GetChartSelect()
            {
                var ret = (from a in ic.Chart
                           where a.Id != (int)DataDefine.DataId.CHART_GAUGE && a.Id != (int)DataDefine.DataId.CHART_CUSTOMIZEDMAP
                                && a.Id != (int)DataDefine.DataId.CHART_SCATTER
                           orderby a.Id
                           select new
                           {
                               a.Id,
                               a.Name,
                               Large = Convert.ToBoolean((int)DataDefine.Width.SIZE_1X3 & a.SizeFlag),
                               Medium = Convert.ToBoolean((int)DataDefine.Width.SIZE_1X2 & a.SizeFlag),
                               Small = Convert.ToBoolean((int)DataDefine.Width.SIZE_1X1 & a.SizeFlag)
                           }).ToArray();
                if (ret.Length == 0)
                    return null;
                return ret;
            }
            //public SelectOptionTemplate[] GetChartSelect()
            //{
            //    var ret = (from a in ic.Chart
            //               where a.Id != 3 && a.Id != 4 && a.Id != 7
            //               orderby a.Id
            //               select new SelectOptionTemplate
            //               {
            //                   Id = a.Id,
            //                   Name = a.Name
            //               }).ToArray();
            //    if (ret.Length == 0)
            //        return null;
            //    return ret;
            //}

            private string StringToTitleCase(string str)
            {
                string[] strArray = str.Split(" ".ToCharArray());
                string ret = string.Empty;

                foreach (string s in strArray)
                {
                    char[] a = s.ToCharArray();

                    a[0] = char.ToUpper(a[0]);
                    ret += new string(a) + " ";
                }

                return ret.TrimEnd();
            }

            public SelectOptionTemplate[] GetMapList()
            {
                var ret = (from a in ic.Chart
                           where (a.DeletedFlag == false) && Convert.ToBoolean(a.Type & 4)
                           orderby a.Id
                           select new SelectOptionTemplate
                           {
                               Id = a.Id,
                               Name = StringToTitleCase(a.Name)
                           }).ToArray();
                if (ret.Length == 0)
                    return null;
                return ret;
            }
            public ChartSizeTemplate GetChartSize(int chartId)
            {
                return (from a in ic.Chart
                        where a.Id == chartId
                        select new ChartSizeTemplate()
                        {
                            Large = Convert.ToBoolean(Convert.ToInt32("100", 2) & a.SizeFlag),
                            Medium = Convert.ToBoolean(Convert.ToInt32("010", 2) & a.SizeFlag),
                            Small = Convert.ToBoolean(Convert.ToInt32("001", 2) & a.SizeFlag)

                        }).SingleOrDefault();
            }

            public ChartSizeTemplate[] GetChartSize()
            {
                return (from a in ic.Chart
                        orderby a.Id
                        where a.Id != (int)DataDefine.DataId.CHART_GAUGE && a.Id != (int)DataDefine.DataId.CHART_CUSTOMIZEDMAP
                        select new ChartSizeTemplate()
                        {
                            Large = Convert.ToBoolean(Convert.ToInt32("100", 2) & a.SizeFlag),
                            Medium = Convert.ToBoolean(Convert.ToInt32("010", 2) & a.SizeFlag),
                            Small = Convert.ToBoolean(Convert.ToInt32("001", 2) & a.SizeFlag)

                        }).ToArray();
            }
            public SelectOptionTemplate[] GetWidgetList()
            {
                return (from a in ic.Widget
                        where a.DeletedFlag == false && a.ChartId != (int)DataDefine.DataId.CHART_CUSTOMIZEDMAP
                        where a.DeletedFlag == false
                        select new SelectOptionTemplate
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).DefaultIfEmpty().ToArray();
            }

            public PanelItemTemplate GetPanelSetting(int widgetId)
            {
                var ret = (from a in ic.Widget
                           join b in ic.Data on a.DataId equals b.Id
                           where a.Id == widgetId && a.DeletedFlag == false
                           select new PanelItemTemplate()
                           {
                               WidgetId = a.Id,
                               DataName = b.Name,
                               DataLocation = b.Location,
                               SettingStr = a.SettingStr,
                               GroupId = b.GroupId,
                               Unit = b.Unit,
                               DataCount = a.DataCount,
                           }).SingleOrDefault();

                if (ret == null)
                {
                    ret = (from a in ic.Widget
                           join b in ic.Threshold on a.ThresholdId equals b.Id
                           join c in ic.Data on b.DataId equals c.Id
                           where a.Id == widgetId && a.DeletedFlag == false
                           select new PanelItemTemplate()
                           {
                               WidgetId = a.Id,
                               DataName = b.Name,
                               DataLocation = null,
                               SettingStr = a.SettingStr,
                               GroupId = c.GroupId,
                               Unit = null,
                               DataCount = a.DataCount,
                               ThresholdId = a.ThresholdId
                           }).SingleOrDefault();

                }

                return ret;
            }

            private class BranchItem
            {
                public int BranchId { get; set; }
                public string BranchName { get; set; }
            }

            public DeviceProfileTemplate[] GetDevice(int widgetId)
            {
                DeviceProfileTemplate[] deviceList = (from a in ic.Widget
                                                      join b in ic.WidgetBranchList on a.Id equals b.WidgetId
                                                      join c in ic.Branch on b.BranchId equals c.Id
                                                      join d in ic.BranchDeviceList on c.Id equals d.BranchId
                                                      join e in ic.Device on d.DeviceId equals e.Id
                                                      join f in ic.Employee on e.OwnerId equals f.Id
                                                      where a.Id == widgetId && a.DeletedFlag == false && e.DeletedFlag == false && c.DeletedFlag == false
                                                      orderby e.Id
                                                      select new DeviceProfileTemplate()
                                                      {
                                                          DevName = e.Name,
                                                          OwnerName = (f.LastName == null || f.LastName.Length == 0) ? ((f.FirstName == null || f.FirstName.Length == 0) ? "" : f.FirstName) : ((f.FirstName == null || f.FirstName.Length == 0) ? f.LastName : f.LastName + " " + f.FirstName),
                                                          Email = f.Email,
                                                          Name = e.Alias ?? e.Name,
                                                          Alias = e.Alias
                                                      }).Distinct().ToArray();
                BranchItem[] branchItem = null;

                foreach (DeviceProfileTemplate device in deviceList)
                {
                    branchItem = (from a in ic.Branch
                                  join b in ic.BranchDeviceList on a.Id equals b.BranchId
                                  join c in ic.Device on b.DeviceId equals c.Id
                                  where c.Name == device.DevName && a.DeletedFlag == false && c.DeletedFlag == false
                                  orderby a.Id
                                  select new BranchItem()
                                  {
                                      BranchId = a.Id,
                                      BranchName = a.Name
                                  }).ToArray();
                    device.BranchId = branchItem.Select(i => i.BranchId).ToArray();
                    device.BranchName = branchItem.Select(i => i.BranchName).ToArray();
                }

                return deviceList;
            }

            public DeviceProfileTemplate GetDevice(string devName)
            {
                DeviceProfileTemplate specificDevice = (from a in ic.Device
                                                        join b in ic.Employee on a.OwnerId equals b.Id
                                                        where a.Name == devName && a.DeletedFlag == false
                                                        select new DeviceProfileTemplate()
                                                        {
                                                            OwnerName = (b.LastName == null || b.LastName.Length == 0) ? ((b.FirstName == null || b.FirstName.Length == 0) ? "" : b.FirstName) : ((b.FirstName == null || b.FirstName.Length == 0) ? b.LastName : b.LastName + " " + b.FirstName),
                                                        }).SingleOrDefault();
                BranchItem[] branchItem = null;

                branchItem = (from a in ic.Branch
                              join b in ic.BranchDeviceList on a.Id equals b.BranchId
                              join c in ic.Device on b.DeviceId equals c.Id
                              where c.Name == devName && a.DeletedFlag == false && c.DeletedFlag == false
                              orderby a.Id
                              select new BranchItem()
                              {
                                  BranchId = a.Id,
                                  BranchName = a.Name
                              }).ToArray();
                specificDevice.BranchId = branchItem.Select(i => i.BranchId).ToArray();
                specificDevice.BranchName = branchItem.Select(i => i.BranchName).ToArray();

                return specificDevice;
            }

            public object GetWidgetNameAndWidth(int widgetId)
            {
                return (from a in ic.Widget
                        where a.Id == widgetId
                        select new
                        {
                            id = a.Id,
                            name = a.Name,
                            width = a.Width
                        }).SingleOrDefault();
            }
        }

        public class _email : IEmail
        {
            private icapContext ic;
            const byte SSL_Encryption = 1;
            const byte TLS_Encryption = 2;
            public _email()
            {
                ic = new icapContext();
            }

            private bool ValidRequest(EmailSettingTemplate e)
            {
                if (e.SMTPAddress == null || e.PortNumber == null || e.EmailFrom == null || e.Password == null || false == IsValidPassword(e.Password))
                    return false;
                return true;
            }

            public bool CteateOrUpdate(EmailSettingTemplate e)
            {
                Email em = ic.Email.FirstOrDefault();
                bool notifyFlag = false;
                if (em == null)
                {
                    if (ValidRequest(e))
                    {
                        try
                        {
                            ic.Email.Add(new Email()
                            {
                                CompanyId = 1,
                                CreatedDate = DateTime.UtcNow,
                                LastModifiedDate = DateTime.UtcNow,
                                SMTPAddress = e.SMTPAddress,
                                PortNumber = e.PortNumber,
                                Encryption = Convert.ToByte(((e.EnableSSL)? SSL_Encryption:0) | ((e.EnableTLS)?TLS_Encryption:0)),
                                EmailFrom = e.EmailFrom,
                                Password = e.Password,
                                Enable = e.Enable,
                                ResendInterval = e.ResendInterval * 60 * 60
                            });

                            ic.SaveChanges();
                            var handler = new NotifyCoreService("172.30.0.13", "N", "SMTP/UPDATE");
                            Thread thread = new Thread(new ThreadStart(handler.Send));
                            thread.Start();
                            return true;
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    else
                        return false;
                }
                else
                {
                    em.CompanyId = 1;
                    em.LastModifiedDate = DateTime.UtcNow;

                    if (e.SMTPAddress != null && e.SMTPAddress != em.SMTPAddress)
                    {
                        em.SMTPAddress = e.SMTPAddress;
                        notifyFlag = true;
                    }
                  
                    if (e.PortNumber != null && e.PortNumber != em.PortNumber)
                    {
                        em.PortNumber = e.PortNumber;
                        notifyFlag = true;
                    }

                    byte encryption = Convert.ToByte(((e.EnableSSL) ? SSL_Encryption : 0) | ((e.EnableTLS) ? TLS_Encryption : 0));
                    if (encryption != em.Encryption)
                    {
                        em.Encryption = encryption;
                        notifyFlag = true;
                    }
            
                    if (e.EmailFrom != null && em.EmailFrom != e.EmailFrom)
                    {
                        em.EmailFrom = e.EmailFrom;
                        notifyFlag = true;
                    }
                        
                    if (e.Password != null && em.Password != e.Password)
                    {
                        em.Password = e.Password;
                        notifyFlag = true;
                    }
                        
                    if (e.Enable != null && em.Enable != e.Enable)
                    {
                        em.Enable = e.Enable;
                        notifyFlag = true;
                    }
                       
                    if (e.ResendInterval != null && em.ResendInterval != e.ResendInterval * 60 * 60)
                    {
                        em.ResendInterval = e.ResendInterval * 60 * 60;
                        notifyFlag = true;
                    }
                        
                    ic.Email.Update(em);
                    try
                    {
                        ic.SaveChanges();
                        
                        if (notifyFlag)
                        {
                            var handler = new NotifyCoreService("172.30.0.13", "N", "SMTP/UPDATE");
                            Thread thread = new Thread(new ThreadStart(handler.Send));
                            thread.Start();
                        }
                        return true;
                    }
                    catch (DbUpdateException)
                    {
                        throw;
                    }
                }
            }

            public List<EmailSettingTemplate> GetEmailList(int? CompanyId)
            {
                try
                {
                    List<Email> emailList = ic.Email.Where(x => x.CompanyId == CompanyId).ToList();
                    List<EmailSettingTemplate> estList = null;

                    if (emailList.Count != 0)
                    {
                        estList = new List<EmailSettingTemplate>();
                    }

                    foreach (Email em in emailList)
                    {
                        estList.Add(new EmailSettingTemplate()
                        {
                            SMTPAddress = em.SMTPAddress,
                            PortNumber = em.PortNumber,
                            EnableSSL = Convert.ToBoolean(SSL_Encryption & em.Encryption),
                            EnableTLS = Convert.ToBoolean(TLS_Encryption & em.Encryption),
                            EmailFrom = em.EmailFrom,
                            Enable = em.Enable,
                            ResendInterval = em.ResendInterval / (60 * 60),
                            Password = null
                        });
                    }

                    return estList;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public int? GetOwnerId(string deviceName)
            {
                Device dev = ic.Device.Where(x => x.Name == deviceName && x.DeletedFlag == false).SingleOrDefault();

                if (dev == null)
                    return null;
                else
                    return dev.OwnerId;
            }

            public bool? Delete(string emailFrom)
            {
                Email em = ic.Email.Where(e => e.EmailFrom == emailFrom).SingleOrDefault();

                if (em == null)
                    return null;
                try
                {
                    ic.Email.Attach(em);
                    ic.Email.Remove(em);
                    ic.SaveChanges();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public bool Send(EmailSendingInfoTemplate emailSendingInfo, int? employeeId)
            {
                Employee emp = ic.Employee.Where(x => x.Id == employeeId).SingleOrDefault();
                Email em = ic.Email.Where(x => x.Enable == true).FirstOrDefault();
                List<Employee> empList = ic.Employee.Where(x => x.CompanyId == emp.CompanyId).ToList();

                if (em == null)
                    return false;
                else
                {
                    try
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("Event Reminder", em.EmailFrom));

                        foreach (Employee ele in empList)
                        {
                            if (ele.Email != null)
                                message.To.Add(new MailboxAddress(ele.LoginName, ele.Email));
                        }
                        message.Subject = "(Warning)Event Happened.Please check out your device.";
                        message.Body = new TextPart("plain") //media type 
                        {
                            Text = "Device Name : " + emailSendingInfo.deviceName + "\r\n" + "Class : " + emailSendingInfo.Class + "\r\n" + "Information : " + emailSendingInfo.Info
                        };

                        using (var client = new SmtpClient())
                        {
                            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                            client.Connect(em.SMTPAddress, (int)em.PortNumber, Convert.ToBoolean(em.Encryption & SSL_Encryption));
                            client.Authenticate(em.EmailFrom, Encoding.UTF8.GetString(Convert.FromBase64String(em.Password)));
                            client.Send(message);
                            client.Disconnect(true);
                        }
                        return true;
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            public void Send(EmailMessageTemplate msgInfo)
            {
                Email em = ic.Email.Where(x => x.Enable == true).FirstOrDefault();
                List<Employee> empList = ic.Employee.Where(e => msgInfo.Id.Contains(e.Id) && e.DeletedFlag == false).ToList();

                if (em == null)
                {
                    throw new Exception("You need to setup your Mail Server on setting page first.");
                }

                if (empList.Count == 0)
                {
                    throw new Exception("Did not find the employee's information");
                }

                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Dashboard Event", em.EmailFrom));

                    foreach (Employee ele in empList)
                    {
                        if (ele.Email != null)
                            message.To.Add(new MailboxAddress(ele.LoginName, ele.Email));
                    }

                    message.Subject = msgInfo.Subject;
                    message.Body = new TextPart("plain") //media type 
                    {
                        Text = msgInfo.Message
                    };

                    using (var client = new SmtpClient())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect(em.SMTPAddress, (int)em.PortNumber, Convert.ToBoolean(em.Encryption & SSL_Encryption));
                        client.Authenticate(em.EmailFrom, Encoding.UTF8.GetString(Convert.FromBase64String(em.Password)));
                        client.Send(message);
                        client.Disconnect(true);
                    }
                }
                catch
                {
                    throw new Exception("Failed to send the message. Please check the mail server settings and the receiving email address.");
                }

            }
            public EmailSettingTemplate GetEmail(int CompanyId)
            {
                try
                {
                    //EmailSettingTemplate email = ic.Email.Where(x => x.CompanyId == CompanyId).FirstOrDefault();

                    var email = (from a in ic.Email
                                 where a.CompanyId == CompanyId
                                 select new EmailSettingTemplate()
                                 {
                                     SMTPAddress = a.SMTPAddress,
                                     PortNumber = a.PortNumber,
                                     EmailFrom = a.EmailFrom,
                                     Password = a.Password,
                                     ResendInterval = a.ResendInterval,
                                     Enable = a.Enable,
                                     EnableSSL = Convert.ToBoolean(a.Encryption & SSL_Encryption),
                                     EnableTLS = Convert.ToBoolean(a.Encryption & TLS_Encryption)
                                 }).FirstOrDefault();

                    if (email == null)
                        return null;
                    return email;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            public List<string> GetEmployeeEmailList(int CompanyId)
            {
                try
                {
                    List<Employee> empList = ic.Employee.Where(x => x.CompanyId == CompanyId).ToList();
                    if (empList.Count == 0)
                    {
                        return (List<string>)null;
                    }

                    List<string> emailList = new List<string>();

                    foreach (Employee e in empList)
                    {
                        if (null != e.Email)
                            emailList.Add(e.Email);
                    }
                    return emailList;
                }
                catch (Exception exe)
                {
                    throw exe;
                }
            }

            public bool IsValidEmail(string emailAddress)
            {
                // Return true if strIn is in valid e-mail format.
                return Regex.IsMatch(emailAddress,
                       @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                       @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
            }

            public bool IsValidMailServerData(EmailTestTemplate emailTestInfo)
            {
                if (null == emailTestInfo ||
                    null == emailTestInfo.EmailFrom ||
                    null == emailTestInfo.Password ||
                    0 == emailTestInfo.PortNumber ||
                    null == emailTestInfo.SMTPAddress ||
                    false == IsValidEmail(emailTestInfo.EmailFrom)
                   )
                {
                    return false;
                }
                if ("" != emailTestInfo.Password && null != emailTestInfo.Password)
                {
                    if (false == IsValidPassword(emailTestInfo.Password))
                        return false;
                }
                return true;
            }

            private string GetPassword()
            {
                return ic.Email.Select(e => e.Password).SingleOrDefault();
            }

            public void Send(string user, EmailTestTemplate emailTestInfo)
            {
                try
                {
                    if ("" == emailTestInfo.Password || null == emailTestInfo.Password)
                    {
                        emailTestInfo.Password = GetPassword();
                    }

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Test Mail", emailTestInfo.EmailFrom));
                    message.To.Add(new MailboxAddress(user, emailTestInfo.EmailTo));
                    message.Subject = "Email Serever Setup";
                    message.Body = new TextPart("plain") //media type 
                    {
                        Text = "Hi,\r\n" +
                        "\r\nIf you're reading this email, it means Mail Server successfully transmitted your test email !" +
                        "\r\n\r\n[This is an automated message generated by iCAP Web Service. Please do not reply to this message]"
                    };

                    using (var client = new SmtpClient())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect(emailTestInfo.SMTPAddress, emailTestInfo.PortNumber, emailTestInfo.EnableSSL);
                        client.Authenticate(emailTestInfo.EmailFrom, Encoding.UTF8.GetString(Convert.FromBase64String(emailTestInfo.Password)));
                        client.Send(message);
                        client.Disconnect(true);
                    }
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            public bool IsValidPassword(string password)
            {
                if (false == CommonFunctions.IsBase64String(password))
                    return false;

                return true;
            }

            public bool IsValidPassword(string password, string verifyPassword)
            {
                if (false == CommonFunctions.IsBase64String(password) ||
                    false == CommonFunctions.IsBase64String(verifyPassword) ||
                   !string.Equals(password, verifyPassword))
                {
                    return false;
                }
                return true;
            }
        }

        public class _multipleDashboard : IMultipleDashboard
        {
            private icapContext ic;
            static List<int> newWidgetList = null;
            public _multipleDashboard()
            {
                ic = new icapContext();
            }

            public int[] GetDashboardIdList(int companyId)
            {
                return (from a in ic.Companydashboard
                        where a.CompanyId == companyId && a.DeletedFlag == false
                        orderby a.Id
                        select a.Id).ToArray();
            }

            public PanelItemTemplate[] GetPanelItem(int DashboardId)
            {
                icapContext ic = new icapContext();

                var ret = (from a in ic.Companydashboardelement
                           join b in ic.Widget on a.WidgetId equals b.Id
                           join c in ic.Data on b.DataId equals c.Id
                           join d in ic.Chart on b.ChartId equals d.Id
                           where a.DashboardId == DashboardId
                           orderby a.IteratorIndex
                           select new PanelItemTemplate()
                           {
                               WidgetId = b.Id,
                               WidgetName = b.Name,
                               WidgetType = d.Name,
                               WidgetWidth = b.Width,
                               DataName = c.Name,
                               DataCount = b.DataCount,
                               DataLocation = c.Location,
                               SettingStr = b.SettingStr
                           }).ToArray();
                if (ret.Length == 0)
                    return new PanelItemTemplate[] { null };
                return ret;
            }
            public string GetDataLocation(int DataId)
            {
                return (from lo in ic.Data
                        where lo.Id == DataId
                        select lo.Location).SingleOrDefault();
            }

            public string[] GetDeviceListByWidgetId(int widgetId)
            {
                string[] devicelist = (from a in ic.Widget
                                       join b in ic.WidgetBranchList on a.Id equals b.WidgetId
                                       join c in ic.Branch on b.BranchId equals c.Id
                                       join d in ic.BranchDeviceList on c.Id equals d.BranchId
                                       join e in ic.Device on d.DeviceId equals e.Id
                                       where a.Id == widgetId && a.DeletedFlag == false && e.DeletedFlag == false
                                       orderby e.Id
                                       select e.Name).ToArray();
                string[] devicelist_Noduplicate = devicelist.Distinct().ToArray();
                return devicelist_Noduplicate;
            }
            public class Devices
            {
                public string Name { get; set; }
                public string Alias { get; set; }
                public string Owner { get; set; }
            }
            class DPCompare : IEqualityComparer<Devices>
            {
                public bool Equals(Devices x, Devices y)
                {
                    if (x == null && y == null)
                        return true;
                    else if (x == null | y == null)
                        return false;
                    else if (x.Name == y.Name)
                        return true;
                    else
                        return false;
                }

                public int GetHashCode(Devices obj)
                {
                    return obj.Name.GetHashCode();
                }
            }
            public object[] GetDeviceNameAliasOwnerListByWidgetId(int widgetId)
            {
                List<Devices> ret = (from a in ic.Widget
                                     join b in ic.WidgetBranchList on a.Id equals b.WidgetId
                                     join c in ic.Branch on b.BranchId equals c.Id
                                     join d in ic.BranchDeviceList on c.Id equals d.BranchId
                                     join e in ic.Device on d.DeviceId equals e.Id
                                     join f in ic.Employee on e.OwnerId equals f.Id
                                     where a.Id == widgetId && a.DeletedFlag == false && e.DeletedFlag == false
                                     orderby e.Id
                                     select new Devices()
                                     {
                                         Name = e.Name,
                                         Alias = e.Alias,
                                         Owner = (f.LastName == null || f.LastName.Length == 0) ? ((f.FirstName == null || f.FirstName.Length == 0) ? "" : f.FirstName) : ((f.FirstName == null || f.FirstName.Length == 0) ? f.LastName : f.LastName + " " + f.FirstName),
                                     }).ToList();

                var distinctRet = ret.Distinct(new DPCompare());

                return distinctRet.ToArray();
            }

            public bool DashboardExist(int dashboardId)
            {
                var dashboard = ic.Companydashboard.Where(d => d.Id == dashboardId && d.DeletedFlag == false).SingleOrDefault();
                if (dashboard != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public bool WidgetInDashboard(int dashboardId, int widgetId)
            {
                return (ic.Companydashboardelement.Where(c => c.DashboardId == dashboardId).Select(c => c.WidgetId).Contains(widgetId));
            }

            public string[] GetDashboardName(int companyId)
            {
                return (from a in ic.Companydashboard
                        where a.CompanyId == companyId && a.DeletedFlag == false
                        orderby a.Id
                        select a.Name).DefaultIfEmpty().ToArray();
            }
            public SelectOptionTemplate[] GetDashboardList()
            {
                return (from a in ic.Companydashboard
                        where a.DeletedFlag != true
                        select new SelectOptionTemplate()
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).DefaultIfEmpty().ToArray();
            }
            public WidgetOption[] GetDashboardWidgetList(int dashboardId)
            {
                var ret = (from a in ic.Companydashboardelement
                           join b in ic.Widget on a.WidgetId equals b.Id
                           where a.DashboardId == dashboardId
                           orderby a.IteratorIndex
                           select new WidgetOption()
                           {
                               Id = a.WidgetId,
                               Name = b.Name,
                               Width = b.Width
                           }).ToArray();
                if (ret.Length == 0)
                    return new WidgetOption[] { null };
                return ret;
            }

            public int[] GetWidgetId(int dashboardId)
            {
                var ret = (from a in ic.Companydashboardelement
                           where a.DashboardId == dashboardId
                           orderby a.IteratorIndex
                           select a.WidgetId).ToArray();
                if (ret.Length == 0)
                    return null;
                return ret;
            }
            
            public void SetWidgetOrder(WidgetOrderTemplate dashboardElement)
            {
                if (dashboardElement.WidgetIdList.Length == 0)
                    throw new Exception();
                try
                {
                    Companydashboardelement[] latestSetting = (from a in ic.Companydashboardelement
                                                                where a.DashboardId == dashboardElement.DashboardId
                                                                orderby a.IteratorIndex
                                                                select a
                                                              ).ToArray();

                    int[] widgetIdList_Noduplicate = dashboardElement.WidgetIdList.Distinct().ToArray();
                    int widgetOrder = 1;
                    bool[] flag = new bool[widgetIdList_Noduplicate.Length];

                    foreach(Companydashboardelement element in latestSetting)
                    {
                        int index = Array.IndexOf(widgetIdList_Noduplicate, element.WidgetId);

                        if (index != -1)
                        {
                            element.IteratorIndex = index + 1; // the widget order
                            flag[index] = true;
                        }
                        else
                        {
                            element.IteratorIndex = widgetIdList_Noduplicate.Length + widgetOrder++;
                        }

                        ic.Companydashboardelement.Update(element);
                    }
                    ic.SaveChanges();
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            public void Create(string dashboardName)
            {
                try
                {
                    ic.Companydashboard.Add(new Companydashboard()
                    {
                        Name = dashboardName,
                        CompanyId = 1,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow
                    });
                    ic.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (dashboardName == null)
                        throw new DbUpdateException("", new Exception("The dashboard name field can NOT be blank!"));
                    throw new DbUpdateException("", new Exception("Create the dashboard failed! Please refresh the web page and try again."));
                }
            }

            private bool WidgetIdExist(int[] widgetIdList_deduped)
            {
                int[] widgetIdList_AdminDB = ic.Widget.Where(b => b.DeletedFlag == false).Select(b => b.Id).ToArray();

                foreach (int Id in widgetIdList_deduped)
                {
                    if (!widgetIdList_AdminDB.Contains(Id))
                        return false;
                }
                return true;
            }

            public bool SaveDashboardInfo(DashboardTemplate dashboardInfo)
            {
                if (dashboardInfo.Name == null || dashboardInfo.Id == 0)
                    throw new Exception();

                const int limitation = 50;


                Companydashboard companyDashboard = ic.Companydashboard.Where(d => d.Id == dashboardInfo.Id && d.DeletedFlag == false).SingleOrDefault();

                Companydashboardelement[] currentSetting = (from a in ic.Companydashboardelement
                                                           where a.DashboardId == companyDashboard.Id
                                                           orderby a.IteratorIndex
                                                           select a).ToArray();

                int[] currentWidgets = (from a in ic.Companydashboardelement
                                        select a.WidgetId).Distinct().ToArray();
                
                int removeCount = 0;
                try
                {
                    if (dashboardInfo.WidgetIdList != null && dashboardInfo.WidgetIdList.Length != 0)
                    {
                        bool[] flag = new bool[dashboardInfo.WidgetIdList.Length];

                        for (int i = 0; i < currentSetting.Length; i++)
                        {
                            int index = Array.IndexOf(dashboardInfo.WidgetIdList, currentSetting[i].WidgetId);

                            if (index != -1)
                            {
                                flag[index] = true;
                                if (removeCount != 0)
                                {
                                    currentSetting[i].IteratorIndex -= removeCount;
                                    ic.Companydashboardelement.Update(currentSetting[i]);
                                }
                            }
                            else
                            {
                                ic.Companydashboardelement.Remove(currentSetting[i]);
                                removeCount++;
                            }
                        }

                        int iteratorIndex = 1;
                        int length = currentSetting.Length - removeCount;
                        newWidgetList = new List<int>();

                        for (int i = 0; i < flag.Length; i++)
                        {
                            if (flag[i] == false)
                            {
                                int widgetId = dashboardInfo.WidgetIdList[i];
                                ic.Companydashboardelement.Add(new Companydashboardelement()
                                {
                                    CreatedDate = DateTime.UtcNow,
                                    DashboardId = dashboardInfo.Id,
                                    DeletedFlag = false,
                                    LastModifiedDate = DateTime.UtcNow,
                                    WidgetId = widgetId,
                                    IteratorIndex = length + iteratorIndex
                                });
                                if (!currentWidgets.Contains(widgetId))
                                {
                                    newWidgetList.Add(widgetId);
                                }
                            }
                        }

                        if (currentWidgets.Length - removeCount + newWidgetList.Count > limitation)
                        {
                            newWidgetList = null;
                            return false;
                        }
                    }
                    else
                    {
                        ic.Database.ExecuteSqlCommand($"DELETE FROM companydashboardelement where DashboardId={companyDashboard.Id};");
                    }
                    companyDashboard.LastModifiedDate = DateTime.UtcNow;
                    companyDashboard.Name = dashboardInfo.Name;
                    ic.Companydashboard.Update(companyDashboard);
                    ic.SaveChanges();

                    if (newWidgetList != null && newWidgetList.Count != 0)
                    {
                        var notify = new NotifyCoreService("172.30.0.16", "D", "WIDGET/ADD"); 
                        Thread thread = new Thread(new ThreadStart(notify.Send));
                        thread.Start();
                    }
                    return true;
                }
                catch (DbUpdateException exc)
                {
                    throw exc;
                }
                catch (Exception exc)
                {
                    if (companyDashboard == null)
                        throw new Exception("The dashboard could NOT be found!");
                    else
                        throw exc;
                }
            }
            public void Delete(int dashboardId)
            {
                Companydashboard companyDashboard = ic.Companydashboard.Where(d => d.Id == dashboardId && d.DeletedFlag == false).SingleOrDefault();

                try
                {
                    ic.Database.ExecuteSqlCommand($"DELETE FROM companydashboardelement where DashboardId={dashboardId};");
                    companyDashboard.LastModifiedDate = DateTime.UtcNow;
                    companyDashboard.DeletedFlag = true;
                    ic.Companydashboard.Update(companyDashboard);
                    ic.SaveChanges();
                }
                catch (Exception)
                {
                    if (companyDashboard == null)
                        throw new Exception("The dashboard could NOT be found!");
                    throw new Exception("Delete the dashboard failed! Please refresh the web page and try again.");
                }
            }

            public string GetNameByDashboardId(int dashboardId)
            {
                return ic.Companydashboard.Where(d => d.Id == dashboardId && d.DeletedFlag == false).Select(d => d.Name).SingleOrDefault();
            }

            public object[] GetMarkerDevices(string guid)
            {
                return (from a in ic.Device
                        join b in ic.MarkerDevicelist on a.Id equals b.DeviceId
                        join c in ic.Employee on a.OwnerId equals c.Id
                        where b.MarkerGuid == guid
                        select new
                        {
                            a.Name,
                            a.Alias,
                            Owner = (c.LastName == null || c.LastName.Length == 0) ? ((c.FirstName == null || c.FirstName.Length == 0) ? "" : c.FirstName) : ((c.FirstName == null || c.FirstName.Length == 0) ? c.LastName : c.LastName + " " + c.FirstName),
                        }).ToArray();
            }
            public class Marker
            {
                public string Guid { get; set; }
                public int X { get; set; }
                public int Y { get; set; }
                public string Name { get; set; }
                public object[] Devices { get; set; }

            }
            public PanelTemplate GetPanelInfo()
            {
                PanelTemplate ret = new PanelTemplate()
                {
                    WidgetSetting = (from a in ic.Widget
                                     join b in ic.Chart on a.ChartId equals b.Id
                                     where ic.Companydashboardelement.Select(x => x.WidgetId).Contains(a.Id) 
                                     && !(a.DataId == null && a.ThresholdId == null)
                                     select new PanelTemplate.WidgetSettingFormat()
                                     {
                                         WidgetId = a.Id,
                                         DataId = a.DataId,
                                         ThresholdId = a.ThresholdId,
                                         WidgetName = a.Name,
                                         ChartType = b.Name,
                                         WidgetWidth = a.Width,
                                         Setting = JsonConvert.DeserializeObject<SettingItemTemplate>(a.SettingStr),
                                     }).ToArray()
                };

                foreach (PanelTemplate.WidgetSettingFormat element in ret.WidgetSetting)
                {
                    if (element.DataId != null)
                    {
                        element.Data = (from a in ic.Data
                                        where a.Id == element.DataId
                                        select new PanelTemplate.WidgetSettingFormat.DataContent
                                        {
                                            Name = a.Name,
                                            Path = a.Location,
                                            Unit = a.Unit
                                        }).SingleOrDefault();

                        if (element.Setting.Func == (int)DataDefine.DataCalculateFunc.Percentage)
                        {
                            int denominatorId = element.Setting.Divider.DenominatorId;
                            element.Denominator = (from a in ic.Data
                                                   where a.Id == denominatorId
                                                   select new PanelTemplate.WidgetSettingFormat.DataContent
                                                   {
                                                       Name = a.Name,
                                                       Path = a.Location,
                                                       Unit = a.Unit,
                                                   }).SingleOrDefault();
                            element.Devices = GetDeviceNameAliasOwnerListByWidgetId(element.WidgetId);
                        }
                        else if (element.Setting.Func == (int)DataDefine.DataCalculateFunc.CustomizedMap)
                        {

                            var markers = (from a in ic.Marker
                                           where a.CustomizedMapId == element.WidgetId
                                           select new Marker
                                           {
                                               Guid = a.PK_Guid,
                                               X = a.OffsetX,
                                               Y = a.OffsetY,
                                               Name = a.Name,
                                           }).ToArray();
                            element.Markers = new object[markers.Length];
                            int index = 0;
                            foreach (Marker marker in markers)
                            {
                                element.Markers[index++] = new
                                {
                                    marker.X,
                                    marker.Y,
                                    marker.Name,
                                    Devices = GetMarkerDevices(marker.Guid)
                                };
                            }
                        }
                        else
                        {
                            element.Devices = GetDeviceNameAliasOwnerListByWidgetId(element.WidgetId);
                        }
                    }
                    else if (element.ThresholdId != null)
                    {
                        var setting = (from a in ic.Threshold
                                       where a.Id == element.ThresholdId
                                       select new AdminDB.Threshold
                                       {
                                           DataId = a.DataId,
                                           DenominatorId = a.DenominatorId,
                                           Func = a.Func,
                                           Value = a.Value
                                       }).SingleOrDefault();

                        element.ThresholdSetting = new
                        {
                            setting.Func,
                            Value = stringToDoubleArray(setting.Value, ','),
                        };

                        element.Data = (from a in ic.Data
                                        where a.Id == setting.DataId
                                        select new PanelTemplate.WidgetSettingFormat.DataContent
                                        {
                                            Name = a.Name,
                                            Path = a.Location,
                                            Unit = a.Unit,
                                        }).SingleOrDefault();

                        if (setting.DenominatorId != null)
                        {
                            element.Denominator = (from a in ic.Data
                                                   where a.Id == setting.DenominatorId
                                                   select new PanelTemplate.WidgetSettingFormat.DataContent
                                                   {
                                                       Name = a.Name,
                                                       Path = a.Location,
                                                       Unit = a.Unit,
                                                   }).SingleOrDefault();
                        }
                        element.Devices = GetDeviceNameAliasOwnerListByWidgetId(element.WidgetId);
                    }
                }
                return ret;
            }

            public PanelTemplate GetNewPanelInfo()
            {
                if (newWidgetList == null || newWidgetList.Count == 0)
                {
                    return null;
                }

                PanelTemplate ret = new PanelTemplate()
                {
                    WidgetSetting = (from a in ic.Widget
                                     join c in ic.Chart on a.ChartId equals c.Id
                                     where newWidgetList.Contains(a.Id)
                                     && !(a.DataId == null && a.ThresholdId == null)
                                     select new PanelTemplate.WidgetSettingFormat()
                                     {
                                         DataId = a.DataId,
                                         ThresholdId = a.ThresholdId,
                                         WidgetId = a.Id,
                                         WidgetName = a.Name,
                                         ChartType = c.Name,
                                         WidgetWidth = a.Width,
                                         Setting = JsonConvert.DeserializeObject<SettingItemTemplate>(a.SettingStr),
                                     }).ToArray()
                };

                foreach (PanelTemplate.WidgetSettingFormat element in ret.WidgetSetting)
                {
                    if (element.DataId != null)
                    {
                        element.Data = (from a in ic.Data
                                        where a.Id == element.DataId
                                        select new PanelTemplate.WidgetSettingFormat.DataContent
                                        {
                                            Name = a.Name,
                                            Path = a.Location,
                                            Unit = a.Unit
                                        }).SingleOrDefault();

                        if (element.Setting.Func == (int)DataDefine.DataCalculateFunc.Percentage)
                        {
                            int denominatorId = element.Setting.Divider.DenominatorId;
                            element.Denominator = (from a in ic.Data
                                                   where a.Id == denominatorId
                                                   select new PanelTemplate.WidgetSettingFormat.DataContent
                                                   {
                                                       Name = a.Name,
                                                       Path = a.Location,
                                                       Unit = a.Unit,
                                                   }).SingleOrDefault();
                            element.Devices = GetDeviceNameAliasOwnerListByWidgetId(element.WidgetId);
                        }
                        else if (element.Setting.Func == (int)DataDefine.DataCalculateFunc.CustomizedMap)
                        {

                            var markers = (from a in ic.Marker
                                           where a.CustomizedMapId == element.WidgetId
                                           select new Marker
                                           {
                                               Guid = a.PK_Guid,
                                               X = a.OffsetX,
                                               Y = a.OffsetY,
                                               Name = a.Name,
                                           }).ToArray();
                            element.Markers = new object[markers.Length];
                            int index = 0;
                            foreach (Marker marker in markers)
                            {
                                element.Markers[index] = new
                                {
                                    marker.X,
                                    marker.Y,
                                    marker.Name,
                                    Devices = GetMarkerDevices(marker.Guid)
                                };
                                //element.Markers[index++] = GetMarkerDevices(marker.Guid);
                            }
                        }
                    }
                    else if (element.ThresholdId != null)
                    {
                        var setting = (from a in ic.Threshold
                                       where a.Id == element.ThresholdId
                                       select new AdminDB.Threshold
                                       {
                                           DataId = a.DataId,
                                           DenominatorId = a.DenominatorId,
                                           Func = a.Func,
                                           Value = a.Value
                                       }).SingleOrDefault();

                        element.ThresholdSetting = new
                        {
                            setting.Func,
                            Value = stringToDoubleArray(setting.Value, ','),
                        };

                        element.Data = (from a in ic.Data
                                        where a.Id == setting.DataId
                                        select new PanelTemplate.WidgetSettingFormat.DataContent
                                        {
                                            Name = a.Name,
                                            Path = a.Location,
                                            Unit = a.Unit,
                                        }).SingleOrDefault();

                        if (setting.DenominatorId != null)
                        {
                            element.Denominator = (from a in ic.Data
                                                   where a.Id == setting.DenominatorId
                                                   select new PanelTemplate.WidgetSettingFormat.DataContent
                                                   {
                                                       Name = a.Name,
                                                       Path = a.Location,
                                                       Unit = a.Unit,
                                                   }).SingleOrDefault();
                        }
                        element.Devices = GetDeviceNameAliasOwnerListByWidgetId(element.WidgetId);
                    }
                }
                newWidgetList = null;
                return ret;
            }
        }

        public class _threshold : IThreshold
        {
            private icapContext ic;
            public _threshold()
            {
                ic = new icapContext();
            }

            public static string[] GetRecipient(icapContext ic, int thresholdId)
            {
                var permissionEmployeeList = (from a in ic.ThresholdPermissionList
                                              join b in ic.Permission on a.PermissionId equals b.Id
                                              join c in ic.Employee on b.Id equals c.PermissionId
                                              where a.ThresholdId == thresholdId && c.DeletedFlag == false && c.Email != null
                                              select c.Email).ToList();


                var employeeRecipient = (from a in ic.ThresholdEmployeeList
                                         join b in ic.Employee on a.EmployeeId equals b.Id
                                         where b.DeletedFlag == false && a.ThresholdId == thresholdId && b.Email != null
                                         select b.Email).ToList();

                var externalRecipient = (from a in ic.ThresholdExternalRecipientList
                                         join b in ic.ExternalRecipient on a.ExternalRecipientId equals b.Id
                                         where a.ThresholdId == thresholdId
                                         select b.Email).ToList();

                return permissionEmployeeList.Union(employeeRecipient).Union(externalRecipient).ToArray();
            }
            private class ThresholdUpdatedHandler : NotifyCoreService
            {
                private string _command = null;
                private string _payload = null;
                private string _ip = null;
                public icapContext _ic = null;
                public string[] _lastRecipient { get; set; }
                public int _thresholdId { get; set; }
                public string[] _lastDevNameList { get; set; }
                public ThresholdUpdatedHandler(string ip, string command, string payload, icapContext ic, int thresholdId): base(ip, command, payload)
                {
                    _ip = ip;
                    _command = command;
                    _payload = payload;
                    _ic = ic;
                    _thresholdId = thresholdId;
                }

                public void CheckRecipient()
                {
                    var _currentRecipient = GetRecipient(_ic, _thresholdId);

                    var q = from a in _lastRecipient
                            join b in _currentRecipient on a equals b
                            select a;

                    bool flag = _lastRecipient.Length == _currentRecipient.Length && q.Count() == _lastRecipient.Length;
                    
                    if (!flag)
                    {
                        // new recipient
                        var notify = new NotifyCoreService(_ip, _command, _payload);
                        notify.Send();
                    }
                }

                public void CheckDevice()
                {
                    var devNameList  = (from a in _ic.ThresholdBranchList
                                             join b in _ic.Branch on a.BranchId equals b.Id
                                             join c in _ic.BranchDeviceList on b.Id equals c.BranchId
                                             join d in _ic.Device on c.DeviceId equals d.Id
                                             where a.ThresholdId == _thresholdId && b.DeletedFlag == false && d.DeletedFlag == false
                                             select d.Name).Distinct().ToArray();

                    bool areEqual = devNameList.SequenceEqual(_lastDevNameList);

                    if (!areEqual)
                    {
                        var notify = new NotifyCoreService(_ip, _command, _payload);
                        notify.Send();
                    }
                }

                public void DeleteCheck()
                {
                    bool isEnable = _ic.Threshold.Where(t => t.Id == _thresholdId).Select(t => t.Enable).SingleOrDefault();

                    if (!isEnable)
                    {
                        return;
                    }

                    var branchDeviceId = (from a in _ic.ThresholdBranchList
                                 join b in _ic.BranchDeviceList on a.BranchId equals b.BranchId
                                 where a.ThresholdId == _thresholdId
                                 select b.Id).FirstOrDefault();

                    if (branchDeviceId > 0)
                    {
                        var notify = new NotifyCoreService(_ip, _command, _payload);
                        notify.Send();
                    }
                }
            }

            public SelectOptionTemplate[] GetThresholdList()
            {
                var ret =
                (
                    from a in ic.Threshold
                    where a.DeletedFlag == false
                    select new SelectOptionTemplate()
                    {
                        Id = a.Id,
                        Name = a.Name
                    }
                ).DefaultIfEmpty().ToArray();

                if (ret.Length == 0)
                    return null;
                else
                    return ret;
            }

            private void AddRecipient(int thresholdId, int[] externalIdArray, string[] externalEmailArray, int[] permissionIdArray, int[] employeeIdArray)
            {
                List<int> employeeIdList = new List<int>();

                if (externalIdArray != null && externalIdArray.Length != 0)
                {
                    var externalRecipientIdList = externalIdArray.Distinct();

                    foreach (int externalRecipientId in externalRecipientIdList)
                    {
                        ic.ThresholdExternalRecipientList.Add(new ThresholdExternalRecipientList()
                        {
                            ThresholdId = thresholdId,
                            ExternalRecipientId = externalRecipientId
                        });
                    }
                }

                if (externalEmailArray != null && externalEmailArray.Length != 0)
                {
                    var externalRecipientList = externalEmailArray.Distinct();
                    int lastId = -1;
                    int count = 0;

                    foreach (string email in externalRecipientList)
                    {
                        // check whether internal email
                        var employeeId = ic.Employee.Where(e => e.DeletedFlag == false && e.Email == email).Select(e => e.Id).SingleOrDefault();
                        if (employeeId == 0)
                        {
                            //check external recipient
                            var externalRecipientId = ic.ExternalRecipient.Where(e => e.Email == email).Select(e => e.Id).SingleOrDefault();

                            if (externalRecipientId == 0)
                            {
                                //the new external recipient email
                                count++;

                                if (lastId == -1)
                                {
                                    lastId = ic.ExternalRecipient.OrderBy(e=>e.Id).Select(e => e.Id).LastOrDefault();
                                }
                                externalRecipientId = lastId + count;

                                ic.ExternalRecipient.Add(new ExternalRecipient()
                                {
                                    Id = externalRecipientId,
                                    Email = email
                                });
                            }

                            ic.ThresholdExternalRecipientList.Add(new ThresholdExternalRecipientList()
                            {
                                ThresholdId = thresholdId,
                                ExternalRecipientId = externalRecipientId
                            });
                        }
                        else
                        {
                            employeeIdList.Add(employeeId);
                        }
                    }
                }

                if (permissionIdArray != null && permissionIdArray.Length != 0)
                {
                    var permissionIdList = permissionIdArray.Distinct();
                    foreach (int permissionId in permissionIdList)
                    {
                        ic.ThresholdPermissionList.Add(new ThresholdPermissionList()
                        {
                            ThresholdId = thresholdId,
                            PermissionId = permissionId
                        });
                    }
                }

                if (employeeIdArray != null && employeeIdArray.Length != 0)
                {
                    employeeIdList = employeeIdList.Union(employeeIdArray.Distinct()).ToList();
                }

                foreach (int employeeId in employeeIdList)
                {
                    ic.ThresholdEmployeeList.Add(new ThresholdEmployeeList()
                    {
                        ThresholdId = thresholdId,
                        EmployeeId = employeeId
                    });
                }
            }


            public void Create(ThresholdTemplate thresholdInfo)
            {
                try
                {
                    var thresholdId = ic.Threshold.OrderBy(t => t.Id).Select(t => t.Id).LastOrDefault() + 1;
                    ic.Threshold.Add(new AdminDB.Threshold()
                    {
                       Id = thresholdId,
                       Name = thresholdInfo.Setting.Name,
                       Value =thresholdInfo.Setting.Value,
                       Func = thresholdInfo.Setting.Func,
                       Enable = true,
                       DenominatorId = thresholdInfo.Setting.DenominatorId,
                       DeletedFlag = false,
                       DataId = thresholdInfo.Setting.DataId,
                       Action = thresholdInfo.Setting.Action,
                       Mode = thresholdInfo.Setting.Mode
                    });

                    AddRecipient(thresholdId, thresholdInfo.ExternalRecipientIdList, thresholdInfo.ExternalRecipientList, thresholdInfo.PermissionIdList, thresholdInfo.EmployeeIdList);

                    ic.SaveChanges();
                }
                catch (DbUpdateException exc)
                {
                    throw exc;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            public bool Update(ThresholdTemplate newSetting)
            {
                AdminDB.Threshold threshold = ic.Threshold.Where(b => b.Id == newSetting.Setting.Id && b.DeletedFlag == false).SingleOrDefault();

                if (threshold == null)
                {
                    return false;
                }
                bool notifyFlag = false;

                if (threshold.Enable)
                {
                    if (threshold.Action != newSetting.Setting.Action)
                    {
                        notifyFlag = true;
                        threshold.Action = newSetting.Setting.Action;
                    }

                    if (threshold.DataId != newSetting.Setting.DataId)
                    {
                        notifyFlag = true;
                        threshold.DataId = newSetting.Setting.DataId;
                    }

                    if (threshold.DenominatorId != newSetting.Setting.DenominatorId)
                    {
                        notifyFlag = true;
                        threshold.DenominatorId = newSetting.Setting.DenominatorId;
                    }

                    if (threshold.Func != newSetting.Setting.Func)
                    {
                        notifyFlag = true;
                        threshold.Func = newSetting.Setting.Func;
                    }

                    if (threshold.Value != newSetting.Setting.Value)
                    {
                        notifyFlag = true;
                        threshold.Value = newSetting.Setting.Value;
                    }
                }
                else
                {
                    threshold.Action = newSetting.Setting.Action;
                    threshold.DataId = newSetting.Setting.DataId;
                    threshold.DenominatorId = newSetting.Setting.DenominatorId;
                    threshold.Func = newSetting.Setting.Func;
                    threshold.Value = newSetting.Setting.Value;
                }
                threshold.Mode = newSetting.Setting.Mode;
                threshold.LastModifiedDate = DateTime.UtcNow;
                threshold.Name = newSetting.Setting.Name;
                ic.Threshold.Update(threshold);
                var handler = new ThresholdUpdatedHandler("172.30.0.13", "N", "TH/UPDATE", ic, newSetting.Setting.Id)
                {
                    _lastRecipient = GetRecipient(ic, threshold.Id),
                };
                try
                {
                    ic.Database.ExecuteSqlCommand($"DELETE FROM thresholdexternalrecipientlist where ThresholdId={newSetting.Setting.Id};");
                    ic.Database.ExecuteSqlCommand($"DELETE FROM thresholdinternalrecipientlist where ThresholdId={newSetting.Setting.Id};");
                    ic.Database.ExecuteSqlCommand($"DELETE FROM thresholdpermissionlist where ThresholdId={newSetting.Setting.Id};");
                    AddRecipient(threshold.Id, newSetting.ExternalRecipientIdList, newSetting.ExternalRecipientList, newSetting.PermissionIdList, newSetting.EmployeeIdList);
                    ic.SaveChanges();
                    if (notifyFlag)
                    {
                        var notify = new NotifyCoreService("172.30.0.13", "N", "TH/UPDATE");
                        Thread thread = new Thread(new ThreadStart(notify.Send));
                        thread.Start();
                    }
                    else
                    {
                        handler._thresholdId = threshold.Id;
                        Thread thread = new Thread(new ThreadStart(handler.CheckRecipient));
                        thread.Start();
                    }
                    return true;
                }
                catch (Exception)
                {
                    throw new Exception("Failed to update the threshold rule. Please refresh the web page try again.");
                }
            }

            public void ModifyWidgetSettingStr(int thresholdId)
            {
                var widgetArray = (from a in ic.Widget
                                   where a.DeletedFlag == false && a.ThresholdId == thresholdId
                                   select a).ToArray();

                foreach (var widget in widgetArray)
                {
                    widget.SettingStr = "{\"Label\":null,\"Func\":-1,\"Divider\":null}";
                    ic.Widget.Update(widget);
                }
            }


            public void Delete(int id)
            {
                AdminDB.Threshold threshold = ic.Threshold.Where(b => b.Id == id && b.DeletedFlag == false).SingleOrDefault();
                if (threshold == null)
                {
                    return;
                }

                try
                {
                    ModifyWidgetSettingStr(id);
                    var notifyFlag = false;
                    bool isEnable = ic.Threshold.Where(t => t.Id == id).Select(t => t.Enable).SingleOrDefault();
                    var branchDeviceId = (from a in ic.ThresholdBranchList
                                          join b in ic.BranchDeviceList on a.BranchId equals b.BranchId
                                          where a.ThresholdId == id
                                          select b.Id).FirstOrDefault();

                    if (branchDeviceId > 0 && isEnable)
                    {
                        notifyFlag = true;
                    }

                    ic.Database.ExecuteSqlCommand($"DELETE FROM thresholdbranchlist where ThresholdId={id};");
                    ic.Database.ExecuteSqlCommand($"DELETE FROM thresholdexternalrecipientlist where ThresholdId={id};");
                    ic.Database.ExecuteSqlCommand($"DELETE FROM thresholdinternalrecipientlist where ThresholdId={id};");
                    ic.Database.ExecuteSqlCommand($"DELETE FROM thresholdpermissionlist where ThresholdId={id};");

                    ic.Threshold.Remove(threshold);
                    threshold.LastModifiedDate = DateTime.UtcNow;
                    //threshold.DeletedFlag = true;
                    //ic.Threshold.Update(threshold);
                    ic.SaveChanges();
                    if (notifyFlag)
                    {
                        var handler = new NotifyCoreService("172.30.0.13", "N", "TH/UPDATE");
                        Thread thread = new Thread(new ThreadStart(handler.Send));
                        thread.Start();
                    }

                }
                catch (Exception)
                {
                    if (threshold == null)
                        throw new Exception("The threshold rule could NOT be found!");
                    throw new Exception("Failed to delete the threshold rule. Please refresh the web page and try again.");
                }
            }

            public SelectOptionTemplate[] GetSelectedGroup(int id)
            {
                var ret = (from a in ic.ThresholdBranchList
                           join b in ic.Branch on a.BranchId equals b.Id
                           where a.ThresholdId == id && b.DeletedFlag == false
                           orderby b.Name.ToUpper()
                           select new SelectOptionTemplate()
                           {
                               Id = b.Id,
                               Name = b.Name
                           }).ToArray();
                if (ret.Length == 0)
                    return null;
                return ret;
            }
            public bool NameExists(string name)
            {
                try
                {
                    var threshold = ic.Threshold.Where(b => b.Name == name && b.DeletedFlag == false).SingleOrDefault();
                    if (threshold != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            public bool NameExists(string name, int id)
            {
                try
                {
                    var threshold = ic.Threshold.Where(b => b.Name == name && b.DeletedFlag == false && b.Id != id).SingleOrDefault();
                    if (threshold != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            public bool IsEnable (int thresholdId)
            {
                return (from a in ic.Threshold
                        where a.Id == thresholdId && a.DeletedFlag == false
                        select a.Enable).Single();
            }
            public bool ThresholdExists(int thresholdId)
            {
                int id = ic.Threshold.Where(t => t.Id == thresholdId && t.DeletedFlag == false).Select(t=>t.Id).SingleOrDefault();

                if (id == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public bool Save(ThresholdTemplate.GroupSetting newSetting)
            {
                AdminDB.Threshold threshold = ic.Threshold.Where(t => t.Id == newSetting.Id && t.DeletedFlag == false).SingleOrDefault();

                if (threshold == null)
                {
                    return false;
                }

                try
                {
                    Thread thread = null;
                    threshold.LastModifiedDate = DateTime.UtcNow;
                    threshold.Enable = newSetting.Enable;
                    ic.Threshold.Update(threshold);
                    var currentSetting = (from a in ic.ThresholdBranchList
                                          where a.ThresholdId == newSetting.Id
                                          select a.BranchId).ToArray();

                    var handler = new ThresholdUpdatedHandler("172.30.0.13", "N", "TH/UPDATE", ic, newSetting.Id)
                    {
                        _lastDevNameList = (from a in ic.ThresholdBranchList
                                            join b in ic.Branch on a.BranchId equals b.Id
                                            join c in ic.BranchDeviceList on b.Id equals c.BranchId
                                            join d in ic.Device on c.DeviceId equals d.Id
                                            where a.ThresholdId == newSetting.Id && b.DeletedFlag == false && d.DeletedFlag == false
                                            select d.Name).Distinct().ToArray()
                    };

                    if (((newSetting.Selected == null || newSetting.Selected.Length == 0) && currentSetting.Length == 0) 
                        || currentSetting.SequenceEqual((newSetting.Selected==null)?new int[0] : newSetting.Selected.Select(t => t.Id).ToArray()))
                    {
                        if (threshold.Enable != false || newSetting.Enable != false)
                        {
                            thread = new Thread(new ThreadStart(handler.Send));
                        }
                    }
                    else
                    {
                        ic.Database.ExecuteSqlCommand($"DELETE FROM thresholdbranchlist where ThresholdId = {newSetting.Id};");
                        if (newSetting.Selected != null)
                        {
                            foreach (SelectOptionTemplate option in newSetting.Selected)
                            {
                                ic.ThresholdBranchList.Add(new ThresholdBranchList()
                                {
                                    BranchId = option.Id,
                                    CreatedDate = DateTime.UtcNow,
                                    DeletedFlag = false,
                                    LastModifiedDate = DateTime.UtcNow,
                                    ThresholdId = newSetting.Id
                                });
                            }
                        }
                        thread = new Thread(new ThreadStart(handler.CheckDevice));
                    }

                    ic.SaveChanges();
                    if (thread != null)
                    {
                        thread.Start();
                    }
                    return true;
                }
                catch (DbUpdateException exc)
                {
                    throw exc;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            private class ThresholdSettingForCoreService
            {
                public int Id { get; set; }
                public int? DenominatorId { get; set; }
                public int Func { get; set; }
                public double[] Value { get; set; }
                public object Data { get; set; }
                public object Denominator { get; set; }
                public bool EnableEmail { get; set; }
                public bool EnableScreenshot { get; set; }
                public string[] EmailList { get; set; }
                public string[] DevNameList { get; set; }              
            };


            public object[] GetSetting()
            {
                ThresholdSettingForCoreService[] thresholds = (from a in ic.Threshold
                                                               join b in ic.Data on a.DataId equals b.Id
                                                               where a.Enable != false && a.DeletedFlag == false
                                                               select new ThresholdSettingForCoreService
                                                               {
                                                                   Id = a.Id,
                                                                   Func = a.Func,
                                                                   Value = stringToDoubleArray(a.Value, ','),
                                                                   Data = new
                                                                   {
                                                                       b.Name,
                                                                       Path = b.Location,
                                                                       b.Unit
                                                                   },
                                                                   DenominatorId = a.DenominatorId,
                                                                   EnableEmail = Convert.ToBoolean(a.Action & 1),
                                                                   EnableScreenshot = Convert.ToBoolean(a.Action & 2),                                          
                                                               }).ToArray();
                List<object> retPayload = new List<object>();

                foreach(ThresholdSettingForCoreService threshold in thresholds)
                {
                    if(threshold.DenominatorId != null)
                    {
                        threshold.Denominator = (from a in ic.Data
                                                 where a.Id == threshold.DenominatorId
                                                 select new
                                                 {
                                                     a.Name,
                                                     Path = a.Location,
                                                     a.Unit
                                                 }).SingleOrDefault();
                    }

                    threshold.DevNameList = (from a in ic.ThresholdBranchList
                                             join b in ic.Branch on a.BranchId equals b.Id
                                             join c in ic.BranchDeviceList on b.Id equals c.BranchId
                                             join d in ic.Device on c.DeviceId equals d.Id
                                             where a.ThresholdId == threshold.Id && b.DeletedFlag == false && d.DeletedFlag == false
                                             select d.Name).Distinct().ToArray();

                    if (threshold.EnableEmail == true)
                    {
                        threshold.EmailList = GetRecipient(ic, threshold.Id);
                    }

                    retPayload.Add(new {
                        threshold.Func,
                        threshold.Value,
                        threshold.EnableEmail,
                        threshold.EnableScreenshot,
                        threshold.Data,
                        threshold.Denominator,
                        threshold.EmailList,
                        threshold.DevNameList
                    });
                }
                return retPayload.ToArray();
            }

            public ThresholdTemplate.WebSetting GetSetting(int thresholdId)
            {
                ThresholdTemplate.WebSetting webSetting = (from a in ic.Threshold
                                             where a.DeletedFlag == false && a.Id == thresholdId
                                             select new ThresholdTemplate.WebSetting()
                                             {
                                                 Setting = new ThresholdTemplate.ThresholdSetting()
                                                 {
                                                     Id = a.Id,
                                                     DataId = a.DataId,
                                                     DenominatorId = a.DenominatorId,
                                                     Value = a.Value,
                                                     Action = a.Action,
                                                     Func = a.Func,
                                                     Name = a.Name,
                                                     Mode = a.Mode
                                                 },
                                             }).SingleOrDefault();

                if (webSetting == null)
                {
                    return null;
                }

                webSetting.PermissionRecipientList = (from a in ic.ThresholdPermissionList
                                                    join b in ic.Permission on a.PermissionId equals b.Id
                                                    where a.ThresholdId == webSetting.Setting.Id
                                                    select new
                                                    {
                                                        PermissionId = b.Id,
                                                        Label = b.Name
                                                    }).ToArray();


                webSetting.EmployeeRecipientList = (from a in ic.ThresholdEmployeeList
                                                 join b in ic.Employee on a.EmployeeId equals b.Id
                                                 where a.ThresholdId == webSetting.Setting.Id && b.DeletedFlag == false
                                                 select new
                                                 {
                                                     EmployeeId = b.Id,
                                                     Label = GetEmailLabel(b.LastName, b.FirstName, b.Email)
                                                 }).ToArray();
                webSetting.ExternalRecipientList = (from a in ic.ThresholdExternalRecipientList
                                                 join b in ic.ExternalRecipient on a.ExternalRecipientId equals b.Id
                                                 where a.ThresholdId == webSetting.Setting.Id 
                                                 select new
                                                 {
                                                     Label = b.Email,
                                                     ExternalRecipientId = b.Id
                                                 }).ToArray();
                return webSetting;
            }

            public AdminDB.Threshold GetThresholdSetting(int thresholdId)
            {
                return (from a in ic.Threshold
                        where a.DeletedFlag == false && a.Id == thresholdId
                        select a).SingleOrDefault();
            }
        }

        public class _data : IData
        {
            private icapContext ic;
            const int DEVICE_LONGITUDE_DATA = 5;
            const int DEVICE_LATITUDE_DATA = 6;
            public _data()
            {
                ic = new icapContext();
            }
            public DataSelect[] GetDataSource()
            {
                DataOption[] dataList =
                (
                    from a in ic.Data
                    join b in ic.DataGroup on a.GroupId equals b.Id
                    where a.Numberical && a.Id != DEVICE_LONGITUDE_DATA && a.Id != DEVICE_LATITUDE_DATA && a.DeletedFlag == false
                    select new DataOption
                    {
                        DataId = a.Id,
                        DataName = a.Name,
                        DataUnit = a.Unit,
                        GroupId = a.GroupId,
                        GroupName = b.Name
                    }
                ).DefaultIfEmpty().ToArray();

                if (dataList.Length == 0)
                {
                    return null;
                }

                List<DataSelect> dataSelect = new List<DataSelect>();
                var dataElement = new DataSelect()
                {
                    GroupOption = new SelectOptionTemplate()
                    {
                        Id = dataList[0].GroupId,
                        Name = dataList[0].GroupName
                    },
                    DataOption = null,
                };
                List<SelectOptionTemplate> options = new List<SelectOptionTemplate>();
                SelectOptionTemplate option = null;

                foreach (DataOption element in dataList)
                {
                    option = new SelectOptionTemplate()
                    {
                        Id = element.DataId,
                        Name = element.DataName,
                        Unit = element.DataUnit
                    };

                    if (element.GroupId != dataElement.GroupOption.Id)
                    {
                        dataElement.DataOption = options.ToArray();
                        dataSelect.Add(dataElement);
                        dataElement = new DataSelect()
                        {
                            GroupOption = new SelectOptionTemplate()
                            {
                                Id = element.GroupId,
                                Name = element.GroupName
                            },
                            DataOption = null,
                        };
                        options = new List<SelectOptionTemplate>();
                        options.Add(option);
                    }
                    else
                    {
                        options.Add(option);
                    }
                }
                dataElement.DataOption = options.ToArray();
                dataSelect.Add(dataElement);
                return dataSelect.ToArray();
            }

            public string GetDataLocation(int dataId)
            {
                return ic.Data.Where(d => d.Id == dataId).Select(d => d.Location).SingleOrDefault();
            }

            public int GetExpiryDate()
            {
                return ic.DeviceRawData.Select(d => d.ExpireDate).FirstOrDefault();
            }
            private class Result
            {
                public int Status { get; set; }
            }

            public bool UpdateExpiryDate(int days)
            {
                NotifyCoreService notifyCoreService = new NotifyCoreService();

                notifyCoreService.Connect("172.30.0.17");
                notifyCoreService.Send("INNO", JsonConvert.SerializeObject(new {
                    Cmd = 8,
                    Enable = (days > 0)? true: false,
                    Days = days
                }));

                string jsonStr = notifyCoreService.ReceiveMessage();

                notifyCoreService.Close();

                var result = JsonConvert.DeserializeObject<Result>(jsonStr);

                if (result.Status == 1)
                {
                    var rawData = ic.DeviceRawData.FirstOrDefault();

                    rawData.ExpireDate = days;
                    rawData.LastModifiedDate = DateTime.UtcNow;
                    ic.DeviceRawData.Update(rawData);

                    try
                    {
                        ic.SaveChanges();
                        return true;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public class _customizedMap : Image, ICustomizedMap
        {
            private icapContext ic;
            public _customizedMap()
            {
                ic = new icapContext();
            }
            public void Create(CustomizedMapTemplate customizedMap)
            {
                try
                {
                    var filename = ContentDispositionHeaderValue
                        .Parse(customizedMap.File.ContentDisposition)
                        .FileName
                        .Trim('"');
                    string ext = filename.Substring(filename.LastIndexOf('.')).ToLower();

                    filename = Convert.ToInt32(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds) + ext;
                    string filePath = "/var/images/maps" + $@"/{filename}";

                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        customizedMap.File.CopyTo(fs);
                        fs.Flush();
                    }
                    // "maps/" + filename,
                    ic.Widget.Add(new Widget
                    {
                        Name = customizedMap.Name,
                        LastModifiedDate = DateTime.UtcNow,
                        CreatedDate = DateTime.UtcNow,
                        SettingStr = $"{{\"Func\":{(int)DataDefine.DataCalculateFunc.CustomizedMap}, \"FilePath\":\"maps/{filename}\"}}",
                        Width = 0b10000,
                        ChartId = (int)DataDefine.DataId.CHART_CUSTOMIZEDMAP,
                        DataCount = 1,
                        DataId = (int)DataDefine.DataId.DATA_DEVICELOCATION,
                        DeletedFlag = false,
                    });
                    ic.SaveChanges();

                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            public SelectOptionTemplate[] GetCustomizedMapList()
            {
                return (from a in ic.Widget
                        orderby a.Id
                        where a.ChartId == (int)DataDefine.DataId.CHART_CUSTOMIZEDMAP
                        select new SelectOptionTemplate()
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).ToArray();
            }

            public object GetMapInfo(int mapid)
            {
                var markers = (from a in ic.Marker
                               where a.CustomizedMapId == mapid
                               select new MarkerCoordinates
                               {
                                   Guid = a.PK_Guid,
                                   Name = a.Name,
                                   OffsetX = a.OffsetX,
                                   OffsetY = a.OffsetY
                               }).ToArray();
                string str = ic.Widget.Where(w => w.Id == mapid).Select(w => w.SettingStr).SingleOrDefault();
                var path = JsonConvert.DeserializeObject<SettingItemTemplate>(str).FilePath;
                if (path == null) return null;
                var imageBytes = System.IO.File.ReadAllBytes($"/var/images/{path}");
                string ext = path.Substring(path.LastIndexOf('.') + 1);
                return new
                {
                    Markers = markers,
                    Image = "data:image/" + ext + ";base64," + Convert.ToBase64String(imageBytes)
                };
            }

            public object GetMarkerDetail(string guid)
            {
                int[] devices = (from a in ic.MarkerDevicelist
                                 where a.MarkerGuid == guid
                                 select a.DeviceId).ToArray();

                //string name = (from a in ic.Marker
                //               where a.PK_Guid == guid
                //               select a.Name).SingleOrDefault();
                //if (name == null) return null;

                return new
                {
                    devices
                };
            }

            public string CreateMarker(MarkerTemplate markerInfo)
            {
                var guid = System.Guid.NewGuid().ToString();
                ic.Marker.Add(new Marker
                {
                    PK_Guid = guid,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow,
                    CustomizedMapId = markerInfo.MapId,
                    Name = markerInfo.Name,
                    OffsetX = markerInfo.OffsetX,
                    OffsetY = markerInfo.OffsetY
                });


                foreach (int id in markerInfo.Devices)
                {
                    ic.MarkerDevicelist.Add(new MarkerDevicelist()
                    {
                        MarkerGuid = guid,
                        DeviceId = id,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow
                    });
                }
                ic.SaveChanges();
                return guid;
            }

            public bool UpdateMarker(string guid, MarkerTemplate markerInfo)
            {
                var marker = ic.Marker.Where(m => m.PK_Guid == guid).Select(m => m).SingleOrDefault();

                if (marker == null) return false;

                marker.Name = markerInfo.Name;
                marker.LastModifiedDate = DateTime.UtcNow;
                marker.OffsetX = markerInfo.OffsetX;
                marker.OffsetY = markerInfo.OffsetY;
                ic.Marker.Update(marker);
                if (markerInfo.Devices != null)
                {
                    ic.Database.ExecuteSqlCommand($"DELETE FROM markerdevicelist where MarkerGuid='{guid}';");
                    foreach (int id in markerInfo.Devices)
                    {
                        ic.MarkerDevicelist.Add(new MarkerDevicelist()
                        {
                            MarkerGuid = guid,
                            DeviceId = id,
                            CreatedDate = DateTime.UtcNow,
                            LastModifiedDate = DateTime.UtcNow
                        });
                    }
                }
                ic.SaveChanges();
                return true;
            }

            public void DeleteMarker(string guid)
            {
                ic.Database.ExecuteSqlCommand($"DELETE FROM marker where PK_Guid='{guid}';");
                ic.SaveChanges();
            }
            public void Update(int mapId, CustomizedMapTemplate customizedMap)
            {
                var map = ic.Widget.Where(w => w.Id == mapId).Select(m => m).SingleOrDefault();

                map.Name = customizedMap.Name;
                if (customizedMap.File != null)
                {
                    var path = JsonConvert.DeserializeObject<SettingItemTemplate>(map.SettingStr).FilePath;
                    ic.Database.ExecuteSqlCommand($"DELETE FROM marker where CustomizedMapId={mapId};");

                    if (System.IO.File.Exists($@"/var/images/{path}"))
                    {
                        // Use a try block to catch IOExceptions, to
                        // handle the case of the file already being
                        // opened by another process.
                        try
                        {
                            System.IO.File.Delete($@"/var/images/{path}");
                        }
                        catch (System.IO.IOException e)
                        {
                            throw e;
                        }
                        var filename = ContentDispositionHeaderValue
                                        .Parse(customizedMap.File.ContentDisposition)
                                        .FileName
                                        .Trim('"');
                        string ext = filename.Substring(filename.LastIndexOf('.')).ToLower();

                        filename = Convert.ToInt32(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds) + ext;
                        string filePath = "/var/images/maps" + $@"/{filename}";

                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            customizedMap.File.CopyTo(fs);
                            fs.Flush();
                        }
                        map.SettingStr = $"{{\"Func\":{(int)DataDefine.DataCalculateFunc.CustomizedMap}, \"FilePath\":\"maps/{filename}\"}}";
                    }
                }
                map.LastModifiedDate = DateTime.UtcNow;
                ic.Widget.Update(map);
                ic.SaveChanges();

            }
            public void DeleteMap(int mapid)
            {
                var map = ic.Widget.Where(m => m.Id == mapid).Select(m => m).SingleOrDefault();

                if (map != null)
                {
                    var path = JsonConvert.DeserializeObject<SettingItemTemplate>(map.SettingStr).FilePath;
                    if (System.IO.File.Exists($@"/var/images/{path}"))
                    {
                        // Use a try block to catch IOExceptions, to
                        // handle the case of the file already being
                        // opened by another process.
                        try
                        {
                            System.IO.File.Delete($@"/var/images/{path}");
                        }
                        catch (System.IO.IOException e)
                        {
                            //Console.WriteLine(e.Message);
                            throw e;
                        }
                    }
                    ic.Widget.Remove(map);
                    ic.SaveChanges();
                }
            }

            public void DeleteAllMarkers(int mapId)
            {
                ic.Database.ExecuteSqlCommand($"DELETE FROM marker where CustomizedMapId={mapId};");
                ic.SaveChanges();
            }

            public string GetMap(string filePath)
            {
                try
                {
                    byte[] imageBytes = System.IO.File.ReadAllBytes("/var/images/" + filePath);
                    string ext = filePath.Substring(filePath.LastIndexOf('.') + 1);

                    return "data:image/" + ext + ";base64," + Convert.ToBase64String(imageBytes);

                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
        }

        public class _permission : IPermission
        {
            private icapContext ic;

            public _permission()
            {
                ic = new icapContext();
            }

            public bool CheckCreatePermission(string loginName, DataDefine.PermissionFlag permission)
            {
                return Convert.ToBoolean((from a in ic.Employee
                                          join b in ic.Permission on a.PermissionId equals b.Id
                                          where a.LoginName == loginName
                                          select b.Create & (int)permission).SingleOrDefault());
            }
            public bool CheckUpdatePermission(string loginName, DataDefine.PermissionFlag permission)
            {
                return Convert.ToBoolean((from a in ic.Employee
                                          join b in ic.Permission on a.PermissionId equals b.Id
                                          where a.LoginName == loginName
                                          select b.Update & (int)permission).SingleOrDefault());
            }
            public bool CheckDeletePermission(string loginName, DataDefine.PermissionFlag permission)
            {
                return Convert.ToBoolean((from a in ic.Employee
                                          join b in ic.Permission on a.PermissionId equals b.Id
                                          where a.LoginName == loginName
                                          select b.Delete & (int)permission).SingleOrDefault());
            }
        }
    }
}

