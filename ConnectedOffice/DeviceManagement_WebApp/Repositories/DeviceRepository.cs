using DeviceManagement_WebApp.Data;
using DeviceManagement_WebApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceManagement_WebApp.Repositories
{
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
}
