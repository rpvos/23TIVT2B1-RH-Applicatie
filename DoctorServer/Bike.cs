using System;
using System.Collections.Generic;
using System.Text;

namespace DoctorApplication
{
    public class Bike
    {
        public string ID { get; set; }

        public Bike(string ID)
        {
            this.ID = ID;
        }

        public override string ToString()
        {
            return this.ID;
        }
    }
}
