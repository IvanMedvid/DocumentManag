using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentManaging.DataAccess.Models
{
    public class Document
    {
        public Document(string userId, string fileName, int size)
        {
            UserId = userId;
            Name = fileName;
            Size = size;
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Size { get; set; }
    }
}
