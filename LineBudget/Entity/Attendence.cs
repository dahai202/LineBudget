using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LineBudget.Entity
{
    class Attendence
    {
        //考勤编号
        public int attendId { get;set;}
        //姓名
        public string staffName { get; set; }
        //部门编号
        public int deptId { get; set; }
        //部门名称
        public string deptName { get;set;}
        //考勤日期
        public DateTime attendDate { get; set; }
        //签到时间
        public string attendIn { get; set; }
        //签退时间
        public string attendOut { get; set; }
        //备注
        public string remark { get; set; }
    }
}
