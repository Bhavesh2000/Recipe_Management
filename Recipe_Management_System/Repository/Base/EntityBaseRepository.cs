//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.ChangeTracking;
//using Recipe_Management_System.AppDbContext;

//namespace Recipe_Management_System.Repository.Base
//{
//    public class EntityBaseRepository<T> : IEntityBaseRepository<T> where T : class, IEntityBase, new()
//    {

//        private readonly ApplicationDbContext _Context;
//        public EntityBaseRepository(ApplicationDbContext Context)
//        {
//            _Context = Context;
//        }
//        public async Task AddAsync(T entity)
//        {
            
//            try
//            {
//                await _Context.Set<T>().AddAsync(entity);
//                await _Context.SaveChangesAsync();
//            }
//            catch (DbUpdateException)
//            {
               
//            }
           
//        }

//        public async Task DeleteAsync(int id)
//        {
//            var entity = await _Context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);
//            EntityEntry entityEntry = _Context.Entry<T>(entity);
//            entityEntry.State = EntityState.Deleted;
//            _Context.SaveChanges();
//        }

//        public  IEnumerable<T> GetAllAsync()
//        {
//            var actors = _Context.Set<T>().ToList();
//            return actors;
//        }

//        public async Task<ActionResult<T>> GetByIdAsync(int id)
//        {
//            var actor = await _Context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);
//            return actor;
//        }

//        public async Task UpdateAsync(int id, T entity)
//        {
//            EntityEntry entityEntry = _Context.Entry<T>(entity);
//            entityEntry.State = EntityState.Modified;
//            _Context.SaveChanges();
//        }

//    }
//}
