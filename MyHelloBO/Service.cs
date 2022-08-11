using System;
using System.Linq;
using System.Collections.Generic;
using EmployeeViewObjects;

namespace MyHelloBO
{
    /// <summary>
    /// My Hello Business Object.
    /// </summary>
    public class Service
    {
        public string GetHelloWorld()
        {
            return "Hi~ 這是第一個 Business Object!!!.";
        }

        public IEnumerable<EmployeeVO> GetEmployees(EmployeeVO input)
        {
            var empIs = input.EmpId;
            var empName = input.EmpName;

            return new EmployeeVO[] {
                new EmployeeVO() { EmpId = 1, EmpName = "gelis", EmpChtName = "吳俊億", Title = "工程師"},
                new EmployeeVO() { EmpId = 1, EmpName = "mary", EmpChtName = "瑪莉", Title = "工程師"}
            };
        }
    }
}
