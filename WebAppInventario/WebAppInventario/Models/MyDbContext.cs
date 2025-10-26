using Microsoft.EntityFrameworkCore;
using WebAppInventario.Models;

namespace WebAppInventario.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext()
        {

        }
        // Constructor con la base de datos 
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<CompraDetalle> ComprasDetalles { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<FacturaDetalle> FacturasDetalles { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>().HasKey(c => c.idCliente);

            // Una categoria puede tener multiples productos
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.idCategoria);

            // Un proveedor puede tener multiples productos
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Proveedor)
                .WithMany(pr => pr.Productos)
                .HasForeignKey(p => p.idProveedor);

            // Un producto tiene un inventario
            modelBuilder.Entity<Inventario>()
                .HasOne(i => i.Producto)
                .WithMany(p => p.Inventario)
                .HasForeignKey(i => i.idProducto);

            // Empleado tiene un Rol
            modelBuilder.Entity<Empleado>()
                .HasOne(i => i.Rol)
                .WithMany(d => d.Empleado)
                .HasForeignKey(i => i.idRol);


            // Un empleado puede tener realizar muchas compras
            modelBuilder.Entity<Compra>()
               .HasOne(i => i.Empleado)
               .WithMany(a => a.Compras)
               .HasForeignKey(i => i.idEmpleado);

            // Una compra puede tener detalles
            modelBuilder.Entity<CompraDetalle>()
                .HasOne(f => f.Compra)
               .WithMany(a => a.ComprasDetalles)
               .HasForeignKey(i => i.idCompra);

            // Una compra puede tener muchos productos comprados -> Inventario para su aumento de stock
            modelBuilder.Entity<CompraDetalle>()
                .HasOne(f => f.Inventario)
               .WithMany(a => a.compraDetalles)
               .HasForeignKey(i => i.idInventario);

            // Un cliente puede tener muchas facturas
            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Cliente)
               .WithMany(a => a.Facturas)
               .HasForeignKey(i => i.idCliente);

            // Un empleado puede generar muchas facturas
            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Empleado)
               .WithMany(a => a.Facturas)
               .HasForeignKey(i => i.idEmpleado);

            // Una factura puede tener muchos detalles
            modelBuilder.Entity<FacturaDetalle>()
               .HasOne(f => f.Facturas)
              .WithMany(a => a.FacturaDetalles)
              .HasForeignKey(i => i.idFactura);

            // Una factura puede tener muchos Productos vendidos
            modelBuilder.Entity<FacturaDetalle>()
               .HasOne(k => k.inventario)
              .WithMany(a => a.facturaDetalles)
              .HasForeignKey(k => k.idInventario);




        }

    }
}
