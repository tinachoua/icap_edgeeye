using ShareLibrary.DataTemplate;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.Interface
{
    public interface ICustomizedMap : IImages
    {
        void Create(CustomizedMapTemplate customizedMap);
        void Update(int mapid, CustomizedMapTemplate customizedMap);
        SelectOptionTemplate[] GetCustomizedMapList();
        object GetMapInfo(int mapid);
        object GetMarkerDetail(string guid);
        string CreateMarker(MarkerTemplate makerInfo);
        bool UpdateMarker(string guid, MarkerTemplate makerInfo);
        void DeleteMarker(string guid);
        void DeleteAllMarkers(int mapId);
        void DeleteMap(int mapid);
        string GetMap(string filePath);
    }
}

