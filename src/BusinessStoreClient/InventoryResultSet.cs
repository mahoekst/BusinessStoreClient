using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessStoreClient
{
    public class InventoryResultSet
    {
        public string ContinuationToken { get; set; }
        public List<InventoryEntryDetails> InventoryEntries { get; set; }
    }
}
