(function () {
    // Inicialización de tooltips
    (() => {
        'use strict';
        const tooltipTriggerList = Array.from(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.forEach(tooltipTriggerEl => {
            new bootstrap.Tooltip(tooltipTriggerEl);
        });
    })();

    // Variables globales
    let detallesVenta = [];
    let numeroFacturaActual = '';
    let debounceCliente, debounceProducto;
    let indexEditar = -1;
    let indexEliminar = -1;

    // Inicializar modales
    let modalEditarProducto, modalEliminarProducto, modalConfirmacionCompra, modalError;

    // Función de inicialización principal
    function inicializar() {
        inicializarCajero();
        obtenerNumeroFactura();
        cargarClientes();
        cargarProductos();
        actualizarFechaHora();

        // Limpiar intervalo previo si existe
        if (window.cajeroInterval) {
            clearInterval(window.cajeroInterval);
        }
        window.cajeroInterval = setInterval(actualizarFechaHora, 1000);

        // Inicializar modales
        modalEditarProducto = new bootstrap.Modal(document.getElementById('modalEditarProducto'));
        modalEliminarProducto = new bootstrap.Modal(document.getElementById('modalEliminarProducto'));
        modalConfirmacionCompra = new bootstrap.Modal(document.getElementById('modalConfirmacionCompra'));
        modalError = new bootstrap.Modal(document.getElementById('modalError'));

        // Eventos de búsqueda
        $('#busquedaCliente').off('input').on('input', filtrarClientes);
        $('#busquedaProducto').off('input').on('input', filtrarProductos);

        // Eventos de modales
        $('#editCantidad').off('input').on('input', actualizarNuevoSubtotal);
        $('#btnConfirmarEdicion').off('click').on('click', confirmarEdicion);
        $('#btnConfirmarEliminacion').off('click').on('click', confirmarEliminacion);
        $('#btnConfirmarCompra').off('click').on('click', procesarPago);

        // Eventos de botones principales
        $('#btnAgregar').off('click').on('click', agregarProducto);
        $('#btnPagar').off('click').on('click', validarPago);
        $('#btnLimpiar').off('click').on('click', confirmarLimpiar);

        // Eventos de selects
        $('#cliente').off('change').on('change', cambioCliente);
        $('#producto').off('change').on('change', cambioProducto);
    }

    // Ejecutar inicialización cuando el DOM esté listo
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', inicializar);
    } else {
        inicializar();
    }

    function inicializarCajero() {
        const nombreEmpleado = localStorage.getItem('empleadoNombre') || 'Sin asignar';
        const partes = nombreEmpleado.trim().split(' ');
        const nombreCorto = partes.length >= 2 ? `${partes[0]} ${partes[partes.length - 2]}` : nombreEmpleado;
        $('#cajeroNombre').text(nombreCorto);
    }

    function obtenerNumeroFactura() {
        $.ajax({
            url: '/api/Facturas/nueva-factura',
            method: 'GET',
            success: function (numeroFactura) {
                numeroFacturaActual = numeroFactura;
                $('#facturaNumero').text(numeroFactura);
            },
            error: function (err) {
                console.error('Error obteniendo número de factura:', err);
                mostrarModalError('Error al obtener el número de factura. Intente recargar la página.');
            }
        });
    }

    function actualizarFechaHora() {
        const ahora = new Date();
        $('#fechaActual').text(ahora.toLocaleDateString('es-SV'));
        $('#horaActual').text(ahora.toLocaleTimeString('es-SV'));
    }

    function cargarClientes() {
        $.ajax({
            url: '/api/Clientes/buscar?buscar=',
            method: 'GET',
            success: function (data) {
                const clientes = data.map(cliente => ({
                    id: cliente.idCliente,
                    nombre: cliente.nombre,
                    email: cliente.email
                }));
                actualizarSelectClientes(clientes);
            },
            error: function (err) {
                console.error('Error cargando clientes:', err);
                mostrarModalError('Error al cargar clientes. Intente nuevamente.');
            }
        });
    }

    function cargarProductos() {
        $.ajax({
            url: '/api/Inventario/buscar-cajero?buscar=',
            method: 'GET',
            success: function (data) {
                const productos = data.map(inv => ({
                    id: inv.idInventario,  // ID de inventario
                    nombre: inv.productoNombre,
                    precio: parseFloat(inv.precio),
                    stock: parseInt(inv.cantidad)  // Asumiendo que la API devuelve 'cantidad' como stock
                }));
                actualizarSelectProductos(productos);
            },
            error: function (err) {
                console.error('Error cargando productos:', err);
                mostrarModalError('Error al cargar productos. Intente nuevamente.');
            }
        });
    }

    function filtrarClientes() {
        clearTimeout(debounceCliente);
        debounceCliente = setTimeout(() => {
            const busqueda = $('#busquedaCliente').val();
            $.ajax({
                url: `/api/Clientes/buscar?buscar=${encodeURIComponent(busqueda)}`,
                method: 'GET',
                success: function (data) {
                    const clientes = data.map(cliente => ({
                        id: cliente.idCliente,
                        nombre: cliente.nombre,
                        email: cliente.email
                    }));
                    actualizarSelectClientes(clientes);
                },
                error: function (err) {
                    console.error('Error filtrando clientes:', err);
                }
            });
        }, 300);
    }

    function filtrarProductos() {
        clearTimeout(debounceProducto);
        debounceProducto = setTimeout(() => {
            const busqueda = $('#busquedaProducto').val();
            $.ajax({
                url: `/api/Inventario/buscar-cajero?buscar=${encodeURIComponent(busqueda)}`,
                method: 'GET',
                success: function (data) {
                    const productos = data.map(inv => ({
                        id: inv.idInventario,  // ID de inventario
                        nombre: inv.productoNombre,
                        precio: parseFloat(inv.precio),
                        stock: parseInt(inv.cantidad)  // Asumiendo que la API devuelve 'cantidad' como stock
                    }));
                    actualizarSelectProductos(productos);
                },
                error: function (err) {
                    console.error('Error filtrando productos:', err);
                }
            });
        }, 300);
    }

    function actualizarSelectClientes(clientes) {
        const select = $('#cliente');
        select.empty().append('<option value="">Seleccione un cliente...</option>');
        if (clientes.length === 0) {
            select.append('<option value="" disabled>No se encontraron resultados</option>');
        } else {
            clientes.forEach(cliente => {
                select.append(`<option value="${cliente.id}" data-email="${cliente.email}">${cliente.nombre}</option>`);
            });
        }
    }

    function actualizarSelectProductos(productos) {
        const select = $('#producto');
        select.empty().append('<option value="">Seleccione un producto...</option>');
        if (productos.length === 0) {
            select.append('<option value="" disabled>No se encontraron resultados</option>');
        } else {
            productos.forEach(producto => {
                select.append(`<option value="${producto.id}" data-precio="${producto.precio}" data-stock="${producto.stock}">${producto.nombre}</option>`);
            });
        }
    }

    function cambioCliente() {
        const email = $('#cliente').find(':selected').data('email') || '';
        $('#email').val(email);
    }

    function cambioProducto() {
        const precio = $('#producto').find(':selected').data('precio') || 0;
        const stock = $('#producto').find(':selected').data('stock') || 0;
        $('#precio').val(precio > 0 ? `${precio.toFixed(2)}` : '');
        // Establecer el max en el input cantidad basado en el stock
        $('#cantidad').attr('max', stock);
    }

    function agregarProducto() {
        const productoId = $('#producto').val();
        const productoNombre = $('#producto option:selected').text();
        const precio = parseFloat($('#producto option:selected').data('precio')) || 0;
        const stock = parseInt($('#producto option:selected').data('stock')) || 0;
        const cantidad = parseInt($('#cantidad').val()) || 1;

        if (!productoId) {
            mostrarModalError('Debe seleccionar un producto');
            return;
        }

        if (cantidad <= 0) {
            mostrarModalError('La cantidad debe ser mayor a 0');
            return;
        }

        // Verificar stock disponible considerando productos ya agregados
        const indiceExistente = detallesVenta.findIndex(d => d.productoId === productoId);
        const cantidadYaAgregada = indiceExistente >= 0 ? detallesVenta[indiceExistente].cantidad : 0;
        const cantidadTotal = cantidadYaAgregada + cantidad;

        if (cantidadTotal > stock) {
            mostrarModalError(`No hay suficiente stock. Disponible: ${stock}, Intentando agregar: ${cantidad} (Total: ${cantidadTotal})`);
            return;
        }

        const subtotal = precio * cantidad;

        if (indiceExistente >= 0) {
            detallesVenta[indiceExistente].cantidad += cantidad;
            detallesVenta[indiceExistente].subtotal = detallesVenta[indiceExistente].precio * detallesVenta[indiceExistente].cantidad;
        } else {
            detallesVenta.push({
                productoId,
                productoNombre,
                precio,
                cantidad,
                subtotal,
                stock  // Agregar stock para referencia en edición
            });
        }

        actualizarTabla();
        limpiarFormularioProducto();
    }

    function limpiarFormularioProducto() {
        $('#producto').val('');
        $('#precio').val('');
        $('#cantidad').val(1);
    }

    function actualizarTabla() {
        const tbody = $('#tablaDetalles tbody');
        tbody.empty();

        if (detallesVenta.length === 0) {
            tbody.html('<tr><td colspan="5" class="text-center text-muted">No hay productos agregados</td></tr>');
            $('#btnPagar').prop('disabled', true);
        } else {
            detallesVenta.forEach((detalle, index) => {
                tbody.append(`
                    <tr>
                        <td>${detalle.productoNombre}</td>
                        <td class="text-end">$${detalle.precio.toFixed(2)}</td>
                        <td class="text-center">${detalle.cantidad}</td>
                        <td class="text-end">$${detalle.subtotal.toFixed(2)}</td>
                        <td class="text-center">
                            <div class="btn-group" role="group">
                                <button class="btn btn-outline-warning btn-sm" onclick="abrirModalEditar(${index})">
                                    <i class="bi bi-pencil-square"></i>
                                </button>
                                <button class="btn btn-outline-danger btn-sm" onclick="abrirModalEliminar(${index})">
                                    <i class="bi bi-trash3"></i>
                                </button>
                            </div>
                        </td>
                    </tr>
                `);
            });
            $('#btnPagar').prop('disabled', false);
        }

        calcularTotales();
    }

    function abrirModalEditar(index) {
        indexEditar = index;
        const detalle = detallesVenta[index];

        $('#editProductoNombre').text(detalle.productoNombre);
        $('#editProductoPrecio').text(detalle.precio.toFixed(2));
        $('#editProductoCantidadActual').text(detalle.cantidad);
        $('#editCantidad').val(detalle.cantidad).attr('max', detalle.stock);  // Establecer max basado en stock

        actualizarNuevoSubtotal();
        modalEditarProducto.show();
    }

    function actualizarNuevoSubtotal() {
        const cantidad = parseInt($('#editCantidad').val()) || 0;
        const precio = parseFloat($('#editProductoPrecio').text()) || 0;
        const nuevoSubtotal = cantidad * precio;
        $('#editNuevoSubtotal').text(nuevoSubtotal.toFixed(2));
    }

    function confirmarEdicion() {
        const nuevaCantidad = parseInt($('#editCantidad').val());
        const detalle = detallesVenta[indexEditar];

        if (nuevaCantidad <= 0) {
            mostrarModalError('La cantidad debe ser mayor a 0');
            return;
        }

        if (nuevaCantidad > detalle.stock) {
            mostrarModalError(`La cantidad no puede exceder el stock disponible: ${detalle.stock}`);
            return;
        }

        detallesVenta[indexEditar].cantidad = nuevaCantidad;
        detallesVenta[indexEditar].subtotal = detallesVenta[indexEditar].precio * nuevaCantidad;

        actualizarTabla();
        modalEditarProducto.hide();
    }

    function abrirModalEliminar(index) {
        indexEliminar = index;
        const detalle = detallesVenta[index];

        $('#delProductoNombre').text(detalle.productoNombre);
        $('#delProductoCantidad').text(detalle.cantidad);
        $('#delProductoSubtotal').text(detalle.subtotal.toFixed(2));

        modalEliminarProducto.show();
    }

    function confirmarEliminacion() {
        detallesVenta.splice(indexEliminar, 1);
        actualizarTabla();
        modalEliminarProducto.hide();
    }

    // Exponer funciones globalmente para los onclick
    window.abrirModalEditar = abrirModalEditar;
    window.abrirModalEliminar = abrirModalEliminar;

    function calcularTotales() {
        const subtotal = detallesVenta.reduce((sum, d) => sum + d.subtotal, 0);
        const iva = subtotal * 0.13;
        const total = subtotal + iva;

        $('#subtotalFactura').text(subtotal.toFixed(2));
        $('#ivaFactura').text(iva.toFixed(2));
        $('#totalFactura').text(total.toFixed(2));
    }

    function validarPago() {
        const tipoPago = $('#tipoPago').val();

        if (!tipoPago) {
            mostrarModalError('Debe seleccionar un tipo de pago');
            return;
        }

        if (detallesVenta.length === 0) {
            mostrarModalError('Debe agregar al menos un producto');
            return;
        }

        // Llenar modal de confirmación
        llenarModalConfirmacion();
        modalConfirmacionCompra.show();
    }

    function llenarModalConfirmacion() {
        const clienteNombre = $('#cliente option:selected').text() || 'Sin cliente';
        const tipoPago = $('#tipoPago option:selected').text();
        const ahora = new Date();

        $('#confFacturaNumero').text(numeroFacturaActual);
        $('#confCliente').text(clienteNombre);
        $('#confTipoPago').text(tipoPago);
        $('#confFecha').text(ahora.toLocaleDateString('es-SV'));
        $('#confHora').text(ahora.toLocaleTimeString('es-SV'));

        const subtotal = detallesVenta.reduce((sum, d) => sum + d.subtotal, 0);
        const iva = subtotal * 0.13;
        const total = subtotal + iva;

        $('#confSubtotal').text(subtotal.toFixed(2));
        $('#confIva').text(iva.toFixed(2));
        $('#confTotal').text(total.toFixed(2));

        const tbody = $('#confProductosTabla');
        tbody.empty();
        detallesVenta.forEach(detalle => {
            tbody.append(`
                <tr>
                    <td>${detalle.productoNombre}</td>
                    <td class="text-end">$${detalle.precio.toFixed(2)}</td>
                    <td class="text-center">${detalle.cantidad}</td>
                    <td class="text-end">$${detalle.subtotal.toFixed(2)}</td>
                </tr>
            `);
        });
    }

    function procesarPago() {
        modalConfirmacionCompra.hide();

        const clienteId = $('#cliente').val();
        const tipoPago = $('#tipoPago').val();

        const subtotal = detallesVenta.reduce((sum, d) => sum + d.subtotal, 0);
        const iva = subtotal * 0.13;
        const total = subtotal + iva;

        const factura = {
            idFactura: 0,
            idEmpleado: parseInt(localStorage.getItem('empleadoId')) || 0,
            idCliente: clienteId ? parseInt(clienteId) : null,
            numeroFactura: numeroFacturaActual.trim(),
            subtotal: subtotal,
            total: total,
            iva: iva,
            metodoPago: tipoPago.charAt(0).toUpperCase() + tipoPago.slice(1),
            fecha: new Date().toISOString().split('T')[0],
            hora: new Date().toTimeString().split(' ')[0]
        };

        // Enviar factura primero
        $.ajax({
            url: '/api/Facturas',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(factura),
            success: function (responseFactura) {
                const idFacturaCreada = responseFactura.idFactura;
                console.log('Factura creada con ID:', idFacturaCreada);

                // Enviar cada detalle individualmente con el idFactura correcto
                let detallesEnviados = 0;
                const totalDetalles = detallesVenta.length;

                detallesVenta.forEach((detalle, index) => {
                    const detalleFactura = {
                        idFacturaDetalle: 0,
                        idFactura: idFacturaCreada,  // Mismo ID de la factura creada
                        idInventario: parseInt(detalle.productoId),  // Siempre ID de inventario
                        cantidad: detalle.cantidad,
                        precio: detalle.precio,
                        subtotal: detalle.subtotal
                    };

                    $.ajax({
                        url: '/api/FacturasDetalles',
                        method: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify(detalleFactura),
                        success: function () {
                            detallesEnviados++;
                            console.log(`Detalle ${detallesEnviados}/${totalDetalles} enviado`);

                            // Disminuir inventario
                            disminuirInventario(detalle.productoId, detalle.cantidad, () => {
                                if (detallesEnviados === totalDetalles) {
                                    mostrarModalError('✅ Pago procesado exitosamente!\nFactura Nº: ' + numeroFacturaActual, true); // Usar modal para éxito también, o alert
                                    limpiarTodo();
                                }
                            });
                        },
                        error: function (err) {
                            console.error(`Error enviando detalle ${index + 1}:`, err);
                            mostrarModalError(`Error al procesar detalle del producto: ${detalle.productoNombre}`);
                        }
                    });
                });
            },
            error: function (err) {
                console.error('Error creando factura:', err);
                mostrarModalError('Error al procesar la factura. Intente nuevamente.');
            }
        });
    }

    function disminuirInventario(idInventario, cantidadVendida, callback) {
        $.ajax({
            url: `/api/Inventario/reducir-stock/${idInventario}?cantidad=${cantidadVendida}`,
            method: 'PUT',
            success: function (response) {
                if (response.mensaje === 'Stock reducido correctamente.') {
                    console.log(`Inventario disminuido para ID ${idInventario} en ${cantidadVendida}. Cantidad restante: ${response.cantidad}`);
                    if (callback) callback();
                } else {
                    mostrarModalError(`Error al reducir stock: ${response.mensaje}`);
                }
            },
            error: function (err) {
                console.error(`Error disminuyendo inventario para ID ${idInventario}:`, err);
                mostrarModalError(`Error al actualizar inventario para el producto con ID ${idInventario}`);
            }
        });
    }

    function confirmarLimpiar() {
        if (confirm('¿Desea limpiar toda la venta actual?')) {
            limpiarTodo();
        }
    }

    function limpiarTodo() {
        detallesVenta = [];
        $('#cliente').val('');
        $('#email').val('');
        $('#tipoPago').val('');
        limpiarFormularioProducto();
        actualizarTabla();
        // Obtener nuevo número de factura
        obtenerNumeroFactura();
    }

    function mostrarModalError(mensaje, esExito = false) {
        $('#errorMensaje').text(mensaje);
        if (esExito) {
            $('#modalErrorLabel').html('<i class="bi bi-check-circle-fill me-2"></i>Éxito');
            $('#modalError .modal-header').removeClass('bg-danger').addClass('bg-success');
        } else {
            $('#modalErrorLabel').html('<i class="bi bi-exclamation-triangle-fill me-2"></i>Error');
            $('#modalError .modal-header').removeClass('bg-success').addClass('bg-danger');
        }
        modalError.show();
    }
})();



        ////