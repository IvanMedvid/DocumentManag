using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentManaging.DataAccess.Models
{
    public class OrderParameters
    {
        public OrderParameters()
        {
        }

        public OrderParameters(string fieldName, bool desc)
        {
            FieldName = fieldName;
            Desc = desc;
        }

        public string FieldName { get; set; } = "Name";
        public bool Desc { get; set; } = false;
    }
}
