namespace TraceWPF.Domain.Models
{
    using System;

    /// <summary>
    /// 雇员实体类
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// 雇员唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 雇员姓名
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 雇员邮箱
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// 雇员电话
        /// </summary>
        public string Phone { get; set; } = "";

        /// <summary>
        /// 所属部门
        /// </summary>
        public string Department { get; set; } = "";

        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; } = "";

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// 薪资
        /// </summary>
        public decimal Salary { get; set; }

        /// <summary>
        /// 是否在职
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
