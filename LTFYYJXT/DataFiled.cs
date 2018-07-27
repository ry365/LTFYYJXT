using System;
using System.Collections.Generic;

namespace LTFYYJXT
{
    public class DataValueList : List<DataValue>
    {
    }

    public class DataValue
    {
        public string Lxdh { get; set; } = "";

        public string Mzh { get; set; }

        public string Sfzh { get; set; } = "";

        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Birthday { get; set; } = "";

        public string Age { get; set; } = "";

        public string Yz { get; set; } = "";

        public string Cbzd { get; set; } = "";

        public string Pgsj { get; set; } = DateTime.Now.ToString("yyyy年MM月dd日");

        public string Pgfj { get; set; } = "";

        public string Bgr { get; set; } = "";

        public string Bgjg { get; set; } = "吉林市龙潭区妇幼保健院";

        public string Bgrq { get; set; } = DateTime.Now.ToString("yyyy年MM月dd日");
    }
}