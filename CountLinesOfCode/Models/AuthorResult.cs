using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountLinesOfCode.Models
{
    public class AuthorResult
    {
        public string Author { get; set; }
        public List<RepositoryResult> Repositories { get; set; }
    }
}
