# WebApp Gestion Inventario
El proyecto consiste en un sistema de gestión de inventario para una ferretería/empresa ficticia (Ferreteria el Maestro).
El sistema permite administrar productos, controlar existencias, gestionar compras y ventas, asignando roles de usuario con diferentes niveles de acceso:

Administrador: acceso total al sistema (gestión de usuarios, clientes, inventario, compras y facturacion).

Bodega: encargado de la gestión de inventario (ingresos, actualizaciones, gestion de productos, proveedores, categorias).

Cajero: responsable de facturación y registro de salidas por ventas (facturacion, registro e devolucion de productos).

El objetivo principal es ofrecer una herramienta sencilla de utilizar y eficiente para el control del inventario y las operaciones diarias de la ferretería.

## Tecnologias a usadas 
- **IDE:**
  Visual Studio 2022
- **Lenguaje:**  C#  
- **Base de datos**  
  SQL Server Express-2019 (de preferencia).
- **API REST FULL**  
- **Repositorio**  Github/Git

#### FrontEnd
- **HTML + CSS + JS**
- **Boostrap**  
  Framework CSS que facilita el diseño responsivo y componentes predefinidos como botones, formularios, tablas y menús.


* **Google Fonts**  
  Servicio que proporciona tipografías modernas y optimizadas para web. Se utilizó para mejorar la estética del texto en las vistas del sistema.
- **LottieFiles**  
   Plataforma que ofrece animaciones ligeras en formato JSON.
  Se usa para animaciones en pantallas de carga, formularios y vistas principales.
- **Favicon.ico** 
  Icono representativo del sistema, mostrado en la pestaña del navegador.
  Se usa para identificar visualmente la aplicación y mejorar su presentación.

- **ChartJs**  
  Librería de JavaScript para crear gráficos interactivos y visualizaciones de datos. Compatible con gráficos de barras, líneas, pastel, radar, etc.
- **Jquery**  
  Librería de JavaScript que simplifica la manipulación del DOM, eventos, animaciones y llamadas AJAX.

#### BackEnd
#### Framework y paquetes NuGet
- **ASP.NET Core**  
  Framework principal para el desarrollo del backend.

- **Microsoft.EntityFrameworkCore.SqlServer (9.0.10)**  
  Permite la integración de Entity Framework Core con SQL Server para gestionar la base de datos de manera ORM.

- **Microsoft.EntityFrameworkCore.Tools (9.0.10)**  
  Herramientas para EF Core que permiten realizar migraciones y otras operaciones desde la línea de comandos o Visual Studio.

- **Microsoft.VisualStudio.Web.CodeGeneration.Design (8.0.7)**  
  Facilita la generación de código scaffolding en proyectos ASP.NET (controladores, vistas, modelos).

- **QuestPDF (2025.7.4)**  
  Librería para generar PDFs dinámicos desde C# de manera sencilla.

- **Swashbuckle.AspNetCore (6.6.2)**  
  Permite integrar Swagger en ASP.NET Core para documentar y probar APIs RESTful.

- **Twilio (7.13.6)**  
  Librería para enviar SMS, realizar llamadas o integrar servicios de comunicación a través de la API de Twilio.

## Instalación y Configuración

Sigue estos pasos para instalar y ejecutar correctamente el sistema de gestión de inventario.

#### 1. Clonar el repositorio

Clona el proyecto desde GitHub:

```bash
git clone https://github.com/tu-usuario/tu-repo.git 
```
#### 2. Crear la base de datos
Crea la base de datos llamada **db_inventario** en:  
***(localdb)\MSSQLLOCALDB***  

#### 3. Ejecutar el script de base de datos
Dentro del proyecto, ve a la carpeta:  
📂 **Db Script/**  
Ejecuta el archivo .sql incluido.
Esto creará todas las tablas, relaciones, datos iniciales y el usuario administrador.

#### 4. Usuario por defecto 
Después de ejecutar el script podrás iniciar sesión con:

Usuario: USR000001  
Contraseña: 123

#### 5. Configuración SMTP (Gmail)
Este apartado permite que el sistema envíe correos electrónicos.

Configura lo siguiente usando User Secrets (por seguridad):
``` json
"SmtpSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "cuentaEjemplo@gmail.com",
  "SenderPassword": "aaaa bbbb cccc dddd",
  "UseSsl": true
}
```
Actualiza los siguientes campos:

SenderEmail → tu correo de Gmail

SenderPassword → contraseña de aplicación (NO la contraseña normal)

UseSsl → debe ser true

SmtpServer y SmtpPort ya están configurados correctamente

****⚠ IMPORTANTE:****
Debes generar una Contraseña de Aplicación en tu cuenta de Google:

Cuenta de Google → Seguridad → Verificación en dos pasos → Contraseñas de aplicación.

Cabe a aclarar que es opcional la configuracion de facturacion, por lo que no hay problema si no se configura el sistema funciona con normalidad.

#### 7. Configuración Twilio (WhatsApp)

El sistema envía alertas de stock bajo por WhatsApp mediante Twilio.

Configura lo siguiente usando User Secrets (por seguridad):
```.json
"TwilioConfig": {
    "AccountSid": "PONER_TU_SID_EN_USER_SECRETS",
    "AuthToken": "PONER_TU_TOKEN_EN_USER_SECRETS",
    "WhatsAppSandboxNumber": "whatsapp:NUMERO_DE_TWILILO",
    "AdminWhatsAppNumber": "PONER_NUMERO_ADMIN_VERIFICADO_EN_USER_SECRETS",
    "StockMinimo": 20
  }
````

Actualiza los siguientes campos:  

AccountSid→ dado por Twilio

AuthToken→ dado por Twilio

WhatsAppSandboxNumber → Numero brindado por Twilio

AdminWhatsAppNumber → Numero autorizado por Twilio para recibir notificaciones

Cabe a aclarar que es opcional esta confirguracion, por lo que no hay problema si no se configura el sistema funciona con normalidad.

Para mas informacion sobre Twilio visita su pajina oficial para obtener las keys.

#### Ejemplo de lo que se recibe de mensaje:  

⚠️ ALERTA DE STOCK BAJO ⚠️  
📦 Producto: {productoNombre}  
📊 Stock actual: {stockActual}    unidades  
Por favor, reponer inventario.

## Uso de aplicación  
  - **Inicio de sesion**  
    Por credencial unica para cada empleado.

- **Inicio**  
  Aqui es el menu principal o slidebar que nos brindara con un mensaje de bienvenida al iniciar sesion donde aqui en adelante los apartados seran habilitados segun el rol del empleado que ingrese.

- **Apartados segun rol**  
  
  * ***Administrador***  
    Acceso total al sistema (gestión de usuarios, clientes, inventario, compras y facturacion, etc).

  * ***Bodega***  
    Encargado de la gestión de inventario (ingresos, actualizaciones, gestion de productos, proveedores y categorias).

  * ***Cajero***  
    Responsable de facturación y registro de salidas por ventas (facturacion, registro de ventas e devolucion de productos).

## Funcionalidades
#### Dashboard  
  Genera reportes a tiempo de cada minuto    
  - Productos Totales
  - Valor de Inventario
  - Productos en stock Bajo
  - Facturas hechas hoy
  - Ventas hechas hoy
  - Valor de Compras por mes
  - Compras Realizadas por mes
  - Numero de devoluciones por mes
  - Numero de proveedores
  - Numero de Clientes
  - Numero de Empleados
  - Numero de Categorías
  Accesos rapidos a nueva compra, factura, producto y cliente-
  - Grafica que muestra las ventas realizadas este mes, con total de ventas y promedio diario
  - Listado de productos mas vendidos
  - Listado de ultimas compras hechas
  - Listado de ultimas facturas hechas
  - Listado de la actividad reciente
  
  


#### Perfil
Se muestra infomacion del empleado que ha ingresado al sistema.

#### Inventario 
- **Productos**  
  CRUD de completo con filtrado de busqueda por nombre, codigo, proveedor y categoria
- **Inventario**  
  busqueda completa con detalle en inventario, filtrado por nombre, codigo, proveedor, categoria y nivel de stock, en caso de stock bajo se envia aviso al administrador via whatsapp 
- **Proveedores**  
  CRUD de completo con busqueda interactiva
- **Categorias**  
  CRUD de completo con busqueda interactiva

#### Compras
- **Realizar compra**  
  Apartado de compra de productos a proveedores para restock.
- **Registro compras**  
  busqueda completa con detalle de compra, filtrado por N° de orden, codigo, proveedor, estado y fecha. Como tambien anulacion de compra.

#### Cajero
- **Facturacion**  
  Apartado de venta de productos a clientes, con facturacion electronica.
- **Facturas**  
  busqueda completa con detalle de factura, filtrado por N° de factura, cliente y fecha.
- **Devoluciones**   
  Apartado para realizar devoluciones de productos por medio de una factura ya hecha.
  busqueda completa con detalle de devolucion, filtrado por N° de factura, empleado y fecha. 


#### Empleados  
CRUD de completo con filtrado de busqueda por nombre, credencial y rol.

#### Clientes  
CRUD de completo con busqueda interactiva.


## Estructura del proyecto

📁 **WebAppInventario**  
├─ 📂 **Connected Services/** — Servicios conectados externos (APIs, servicios web)  
├─ 📂 **Dependencias/** — Paquetes NuGet y librerías del proyecto  
├─ 📂 **Properties/** — Propiedades y configuración del proyecto    
├─ 📂**wwwroot/**  
│   ├─ 📂 ***css/*** —Archivos de estilos  
│   │   ├─ 📂 animations/ — En formato .json   
│   │   └─ 📂 img/ — Imagenes e iconos  
│   ├─ 📂 ***js/***— Funcionalidades para HTML  
│   └─ 📂 ***views/*** — Vistas/formularios HTML  
├─ 📂 **Controllers/** — Controladores de la aplicación (manejo de rutas y lógica de petición/respuesta)  
├─ 📂 **Data/** — Configuración y contexto de base de datos  
├─ 📂 **Db Script/** — Scripts SQL para creación o mantenimiento de la base de datos  
├─ 📂 **docs/** — Esquema u documentacion de base de datos, etc.  
├─ 📂 **Models/** — Clases y estructuras de datos  
├─ 📂 **Services/** — Lógica de negocio y servicios internos  
├─ 📄 **appsettings.json** — Configuración general del proyecto, incluyendo conexión a la base de datos  
├─ 📄 **Program.cs** — Punto de entrada de la aplicación  
└─ 📄 **WeatherForecast.cs** — Clase de ejemplo (puede eliminarse o modificarse según el proyecto)



## Endpoints (API REST)
#### Categorias

    GET     /api/Categorias
    POST    /api/Categorias
    GET     /api/Categorias/buscar
    GET     /api/Categorias/{id}
    PUT     /api/Categorias/{id}
    DELETE  /api/Categorias/{id}

#### Clientes
    GET     /api/Clientes
    POST    /api/Clientes
    GET     /api/Clientes/buscar
    GET     /api/Clientes/{id}
    PUT     /api/Clientes/{id}
    DELETE  /api/Clientes/{id}

#### Compras
    GET     /api/Compras
    POST    /api/Compras
    GET     /api/Compras/buscar
    GET     /api/Compras/buscar-por-fecha
    GET     /api/Compras/buscar-por-proveedor
    PUT     /api/Compras/anular-auto/{id}
    GET     /api/Compras/buscar-anidado
    GET     /api/Compras/nueva-compra
    GET     /api/Compras/{id}
    PUT     /api/Compras/{id}
    DELETE  /api/Compras/{id}

#### Compras Detalles
    GET     /api/ComprasDetalles
    POST    /api/ComprasDetalles
    GET     /api/ComprasDetalles/por-compra/{idCompra}
    GET     /api/ComprasDetalles/{id}
    PUT     /api/ComprasDetalles/{id}
    DELETE  /api/ComprasDetalles/{id}

#### Dashboard
    GET     /api/Dashboard/kpis
    GET     /api/Dashboard/ventas-mensuales
    GET     /api/Dashboard/actividad-reciente
    GET     /api/Dashboard/top-productos
    GET     /api/Dashboard/ultimas-compras
    GET     /api/Dashboard/ultimas-facturas

#### Devoluciones
    GET     /api/Devoluciones
    POST    /api/Devoluciones
    GET     /api/Devoluciones/buscar-devoluciones
    GET     /api/Devoluciones/filtrar-anidado
    GET     /api/Devoluciones/buscar-por-fecha
    GET     /api/Devoluciones/{id}
    PUT     /api/Devoluciones/{id}
    DELETE  /api/Devoluciones/{id}

#### Devoluciones Detalles
    GET     /api/DevolucionesDetalles
    POST    /api/DevolucionesDetalles
    GET     /api/DevolucionesDetalles/{id}
    PUT     /api/DevolucionesDetalles/{id}
    DELETE  /api/DevolucionesDetalles/{id}
    GET     /api/DevolucionesDetalles/por-factura/{idFactura}
    GET     /api/DevolucionesDetalles/por-devolucion/{idDevolucion}

#### Empleados
    GET     /api/Empleados
    POST    /api/Empleados
    GET     /api/Empleados/buscar
    GET     /api/Empleados/filtrar-anidado
    GET     /api/Empleados/nueva-credencial
    POST    /api/Empleados/login
    GET     /api/Empleados/{id}
    PUT     /api/Empleados/{id}
    DELETE  /api/Empleados/{id}


#### Facturas
    GET     /api/Facturas
    POST    /api/Facturas
    GET     /api/Facturas/buscar
    GET     /api/Facturas/filtrar-anidado
    GET     /api/Facturas/buscar-para-devolucion
    GET     /api/Facturas/{id}
    PUT     /api/Facturas/{id}
    DELETE  /api/Facturas/{id}
    GET     /api/Facturas/nueva-factura

#### Facturas Detalles
    GET     /api/FacturasDetalles
    POST    /api/FacturasDetalles
    GET     /api/FacturasDetalles/por-idFactura/{idFactura}
    GET     /api/FacturasDetalles/{id}
    PUT     /api/FacturasDetalles/{id}
    DELETE  /api/FacturasDetalles/{id}

#### Inventario
    GET     /api/Inventario
    POST    /api/Inventario
    GET     /api/Inventario/filtrar-anidado
    GET     /api/Inventario/buscar-cajero
    GET     /api/Inventario/compra-proveedor
    PUT     /api/Inventario/reducir-stock/{id}
    PUT     /api/Inventario/aumentar-stock/{id}
    GET     /api/Inventario/buscar
    GET     /api/Inventario/{id}
    PUT     /api/Inventario/{id}
    DELETE  /api/Inventario/{id}

#### Invoicing (WhatsApp / SMS / Notificaciones)
    POST    /api/Invoicing/send

#### Productos
    GET     /api/Productos
    POST    /api/Productos
    GET     /api/Productos/nuevo-Codigo
    GET     /api/Productos/filtrar-anidado
    GET     /api/Productos/buscar
    GET     /api/Productos/{id}
    PUT     /api/Productos/{id}
    DELETE  /api/Productos/{id}

#### Proveedores
    GET     /api/Proveedores
    POST    /api/Proveedores
    GET     /api/Proveedores/buscar
    GET     /api/Proveedores/{id}
    PUT     /api/Proveedores/{id}
    DELETE  /api/Proveedores/{id}

#### WeatherForecast (demo)
    GET     /WeatherForecast


## Base de Datos (Resumen)
#### roles  
 Define los niveles de acceso dentro del sistema.

    idRol (PK)

    rol

#### empleados

Almacena la información de los usuarios del sistema.

    idEmpleado (PK)

    idRol (FK → roles.idRol)

    nombre

    credencial

    contraseña

    telefono

    email

    direccion

    fechaNacimiento

    estado

    ultimaActualizacion

#### clientes

Almacena la información de los cleintes del sistema.

    idCliente (PK)

    nombre

    telefono

    email

    direccion

    estado

    ultimaActualizacion

#### proveedores

Registra los proveedores de productos.

    idProveedor (PK)

    nombre

    telefono

    email

    direccion

    estado

    ultimaActualizacion

#### categorías

Clasifica los productos.

    idCategoria (PK)

    nombre

    estado

    ultimaActualizacion

#### productos

Contiene la información del producto.

    idProducto (PK)

    idCategoria (FK → categorias.idCategoria)

    idProveedor (FK → proveedores.idProveedor)

    codigo

    nombre

    descripcion

    estado

    fechaProd

    fechaVenc

#### inventario

Contiene la información del inventario.

    idInventario (PK)

    idProducto (FK → productos.idProductos)

    precio 
    
    costo 

    cantidad

    ubicacion

    ultimaActualizacion

#### facturas

Encabezado de cada venta realizada.

    idFactura (PK)

    idEmpleado (FK → empleados.idEmpleado)
    
    idCliente (FK → clientes.idCliente)

    numeroFactura

    subtotal 

    total

    iva

    metodoPago

    fecha

    hora

#### facturasDetalles

Detalle de productos vendidos por factura.

    idFacturaDetalle (PK)

    numeroFactura (FK → Facturas.numeroFactura)

    idInventario (FK → inventario.idInventario)

    cantidad

    precio 

    subtotal


#### devoluciones

Encabezado de cada devolucion realizada.

    idDevolucion (PK)

    idFactura (FK → facturas.idFactura)
    
    idEmpleado (FK → empleados.idEmpleado)

    cantidad

    fechaDevoloucion

    horaDevolucion

    totalDevolucion 


#### devolucionesDetalles

Detalle de productos devueltos por factura.

    idDevokcuionDetalle (PK)

    idDevolucion (FK → devoluciones.idDevolucion)

    idFactura (FK → facturas.idFactura)
    
    idFacturaDetalle (FK → facturasDetalles.idFacturaDetalle)

    cantidadDevuelta

    motivo

    descripcion

    precioUnitario

    subtotal

    reintegrarInventario

#### compras 
Encabezado de cada compra a proveedor realizada.  

    idCompra (PK)

    idEmpleado (FK → empleados.idEmpleado)

    idProveedor (FK → proveedores.idProveedor)

    numeroCompra

    fechaCompra

    horaCompra

    subtotal

    iva

    total

    metodoPago

    cantidad

    estado

    motivoAnulacion

    fechaAnulacion

    idEmpleadoAnulacion (FK → empleados.idEmpleado)

#### comprasDetalles
Detalle de compra a proveedor.

    idCompraDetalle (PK)

    idCompra (FK → compras.idCompra)

    idInventario (FK → inventario.idInventario)

    cantidad

    precio

    costo

    subtotal

    costoAnterior

    precioAnterior


## Colaboradores

- **Jimmy Roberto** — Backend & Base de datos  
  GitHub: https://github.com/jimrobert796

- **Wilfredo Ortez** — Frontend / Diseño UI  
  GitHub: https://github.com/WilfredoOrtez

- **Rodrigo Baires** — Soporte técnico / tecnologia SMTP  
  GitHub:  https://github.com/bairesrodrigo

- **Jaime Salmeron** — Soporte técnico / tecnologia Twilio  
  GitHub:  https://github.com/JaimeSalmeron4k

- **Herber Salgado** — Testing / Maquetacion de ideas  
  GitHub:  https://github.com/JR88GG

- **Israel Morales** — Testing / Mockup  
  GitHub:  https://github.com/israel-morales-gomez


## Licencia

Este proyecto está distribuido bajo la licencia MIT.

Esto significa que cualquier persona puede usar, copiar, modificar, fusionar, publicar, distribuir, sublicenciar y/o vender copias de este software, siempre y cuando se incluya el aviso de copyright original y esta nota de permiso en todas las copias o partes sustanciales del software.

Para más detalles, consulta el archivo `LICENSE` incluido en este repositorio.
