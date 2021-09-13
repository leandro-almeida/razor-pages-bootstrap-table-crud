using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesBootstrapTable.Data;
using RazorPagesBootstrapTable.Grid;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace RazorPagesBootstrapTable.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        

        public async Task<JsonResult> OnGetDadosGrid(GridParams gridParams)
        {
            var recordsTotal = _context.Customers.Count();

            var customersQuery = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(gridParams.Search))
            {
                var searchText = gridParams.Search.ToUpper();
                customersQuery = customersQuery.Where(s =>
                    s.Name.ToUpper().Contains(searchText) ||
                    s.PhoneNumber.ToUpper().Contains(searchText) ||
                    s.Address.ToUpper().Contains(searchText) ||
                    s.PostalCode.ToUpper().Contains(searchText)
                );
            }

            var recordsFiltered = customersQuery.Count();

            if (!string.IsNullOrWhiteSpace(gridParams.Sort))
            {
                var order = $"{gridParams.Sort} {gridParams.Order ?? "asc"}";
                customersQuery = customersQuery.OrderBy(order);
            }

            var skip = gridParams.Offset;
            var take = gridParams.Limit;
            var data = await customersQuery
                .Skip(skip).Take(take)
                .ToListAsync();

            return new JsonResult(new GridResult
            {
                Total = recordsFiltered,
                TotalNotFiltered = recordsTotal,
                Rows = data
            });
        }
    }
}
