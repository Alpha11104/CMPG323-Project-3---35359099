# CMPG323 Project 3

## Usage:
Kindly navigate to https://cmpg323-project-3-35359099.azurewebsites.net/

Create an account or login with the following provided details:

email: user@user.com
password: P@ssword123

Implementation of the Repository Pattern

IGenericRepository
```C#
public interface IGenericRepository<T> where T : class
{
    T GetById(Guid? id);
    void Update(T entity);
    IEnumerable<T> GetAll();
    IEnumerable<T> Find(Expression<Func<T, bool>> expression);
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
```

### GenericRepository:

I changed the method signature to virtual which allows me to override to methods in subclasses which inherit the GenericRepository class.

```C#
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ConnectedOfficeContext _context;

    public GenericRepository(ConnectedOfficeContext context)
    {
        _context = context;
    }
    public virtual void Add(T entity)
    {
        _context.Set<T>().Add(entity);
        _context.SaveChanges();
    }
    public virtual void Update(T entity)
    {
        _context.Set<T>().Update(entity);
        _context.SaveChanges();
    }
    public virtual void AddRange(IEnumerable<T> entities)
    {
        _context.Set<T>().AddRange(entities);
        _context.SaveChanges();
    }
    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> expression)
    {
        return _context.Set<T>().Where(expression);
    }
    public virtual IEnumerable<T> GetAll()
    {
        return _context.Set<T>().ToList();
    }
    public virtual T GetById(Guid? id)
    {
        return _context.Set<T>().Find(id);
    }
    public virtual void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
        _context.SaveChanges();
    }
    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
        _context.SaveChanges();
    }
}
```

### Repository Interfaces

I created an repository interface for each data model in the project. IZoneRepository, ICategoryRepository and IDeviceRepository. However the majority of these interfaces are empty because they need not implement their own methods only inherit their methods from the generic repository interface. 

The exception is IDeviceRepository because the implementation required specific methods changes.

### IDeviceRepository 

```C#
public interface IDeviceRepository: IGenericRepository<Device>
{
    new Device GetById(Guid? id);
    new IEnumerable<Device> GetAll();
    IEnumerable<Category> GetCategories();
    IEnumerable<Zone> GetZones();
}
```

### Repository Classes

I created a repository class for each interface created in the previous step. The majority of these are also empty because they need not implement any special methods outside of those already implemented in the GenericRepository class.

The exception is DeviceRepository because the methods implementation must be changed.

### DeviceRepository 

```C#
public class DeviceRepository : GenericRepository<Device>, IDeviceRepository
{
    protected readonly ConnectedOfficeContext _context;
    public DeviceRepository(ConnectedOfficeContext context) : base(context)
    {
        _context = context;
    }
    public override IEnumerable<Device> GetAll()
    {
        var devices = _context.Device
            .Include(d => d.Category)
            .Include(d => d.Zone)
            .ToList();
        return devices;
    }
    public override Device GetById(Guid? id)
    {
        return _context.Device
            .Include(d => d.Category)
            .Include(d => d.Zone)
            .FirstOrDefault(m => m.DeviceId == id);
    }
    public IEnumerable<Category> GetCategories()
    {
        return _context.Category.ToList();
    }
    public IEnumerable<Zone> GetZones()
    {
        return _context.Zone.ToList();
    }
}
```

The methods listed above were added to the DeviceRepository. The methods GetAll() and GetById() have the same signature as those in the GenericRepository class however because the generic repository class specifies these methods as virtual we can inherit and override them.

### Controllers

All controllers for the web app only reference the repository class which is unique to each data model. No database access or mutation operations are implemented in the controllers. They are only implemented within the GenericRepository class or the specific repository class.

### DeviceController
```C#
public class DevicesController : Controller
    {
        //private readonly ConnectedOfficeContext _context;
        private readonly IDeviceRepository _deviceRepository;
        public DevicesController(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        // GET: Devices
        public async Task<IActionResult> Index()
        {
            var devices = _deviceRepository.GetAll();
            return View(devices);
        }

        // GET: Devices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = _deviceRepository.GetById(id);

            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // GET: Devices/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_deviceRepository.GetCategories(), "CategoryId", "CategoryName");
            ViewData["ZoneId"] = new SelectList(_deviceRepository.GetZones(), "ZoneId", "ZoneName");
            return View();
        }

        // POST: Devices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeviceId,DeviceName,CategoryId,ZoneId,Status,IsActive,DateCreated")] Device device)
        {
            device.DeviceId = Guid.NewGuid();
            _deviceRepository.Add(device);
         
            return RedirectToAction(nameof(Index));


        }

        // GET: Devices/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = _deviceRepository.GetById(id);

            if (device == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_deviceRepository.GetCategories(), "CategoryId", "CategoryName", device.CategoryId);
            ViewData["ZoneId"] = new SelectList(_deviceRepository.GetZones(), "ZoneId", "ZoneName", device.ZoneId);
            return View(device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("DeviceId,DeviceName,CategoryId,ZoneId,Status,IsActive,DateCreated")] Device device)
        {
            if (id != device.DeviceId)
            {
                return NotFound();
            }
            try
            {
                _deviceRepository.Update(device);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(device.DeviceId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: Devices/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = _deviceRepository.GetById(id);

            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var device = _deviceRepository.GetById(id);
            _deviceRepository.Remove(device);
            
            return RedirectToAction(nameof(Index));
        }

        private bool DeviceExists(Guid id)
        {
            var device = _deviceRepository.GetById(id);

            if(device == null)
            {
                return false;
            }
            return true;
        }
    }
```

### Dependency Injection

The following dependencies are added in the startup class in order to instatiate the repositories. With this implementation there is only a single class ConnectedOffice.cs which has direct access to the database.

### Startup class
```C#
    services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    services.AddTransient<ICategoryRepository, CategoryRepository>();
    services.AddTransient<IZoneRepository, ZoneRepository>();
    services.AddTransient<IDeviceRepository, DeviceRepository>();
```