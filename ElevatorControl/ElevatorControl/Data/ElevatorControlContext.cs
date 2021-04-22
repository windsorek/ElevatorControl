using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ElevatorControl.Models;
using System.Collections.Concurrent;

namespace ElevatorControl.Data
{
    public class ElevatorControlContext : DbContext
    {
        /// <summary>
        /// niby zero references ale jednak przy wywołaniu metody tutaj lądujemy
        /// </summary>
        /// <param name="options"></param>
        public ElevatorControlContext(DbContextOptions<ElevatorControlContext> options)
            : base(options)
        {
        }

        public DbSet<ElevatorControl.Models.Elevator> Elevators { get; private set; }

    }
}
