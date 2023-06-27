using DoGiaDung.Models;

namespace DoGiaDung.Library
{
    public class CheckSlug
    {
        private readonly WebbangiadungContext _context;
        public CheckSlug(WebbangiadungContext context)
        {
            _context = context;
        }

        public bool KiemTraSlug(String Table, String Slug, int? id)
        {
            switch(Table)
            {
                case "Category":
                    if(id != null)
                    {
                        if (_context.Categories.Where(m => m.Slug == Slug && m.Id == id).Count() > 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (_context.Categories.Where(m => m.Slug == Slug).Count() > 0)
                            return false;
                    }
                    break;
                case "Topic":
                    break;
                case "Post":
                    break;
                case "Product":
                    break;
            }
            return true;
        }
    }
}
