using System;

namespace EmployeeViewObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class EmployeeVO
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string EmpChtName { get; set; }
        public string Title { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
