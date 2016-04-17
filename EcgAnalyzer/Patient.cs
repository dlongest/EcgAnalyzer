using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{   
    public class Patient
    {
        private readonly string identifier;

        public Patient(string identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier");

            if (identifier.Length == 0)
                throw new ArgumentException("identifier must be provided");

            this.identifier = identifier;
        }        
    }
   
}
