using System.ComponentModel;

namespace Member.Models
{
    public class Member1
    {
        public string pk { get; set; }
        [DisplayName("帳號")]
        public string id { get; set; }
        [DisplayName("姓名")]
        public string name { get; set; }
        [DisplayName("性別")]
        public string gender { get; set; }
        [DisplayName("生日")]
        public string birthday { get; set; }
        [DisplayName("密碼")]
        public string pwd { get; set; }
        [DisplayName("備註")]
        public string remark { get; set; }
        [DisplayName("啟用")]
        public string enable { get; set; }
    }
}
