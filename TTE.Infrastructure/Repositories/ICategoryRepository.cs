using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Infrastructure.Models;

namespace TTE.Infrastructure.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategories();
    }
}
