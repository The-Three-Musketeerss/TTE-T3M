using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    public class Job
    {
        public int Id { get; set; }
        public int Item_id { get; set; }
        public DateTime CreatedAt { get; set; }

        //Type
        public enum JobEnum { Product, Category }
        public JobEnum Type { get; set; }

        //Operation
        public enum OperationEnum { Create, Delete }
        public OperationEnum Operation { get; set; }

        //Status
        public enum StatusEnum { Approved, Declined }
        public StatusEnum Status { get; set; }
    }
}
