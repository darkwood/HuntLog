using System;
namespace HuntLog.Cells
{
    public class CellAction
    {
        public Action<object> Save { get; set; }
        public Action Delete { get; set; }

    }
}
