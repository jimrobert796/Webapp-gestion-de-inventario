(function () {
    let empleados = [];
    let filaSeleccionada = null;
    let modo = 'normal'; // Estados: 'normal', 'agregar', 'editar'

    function renderTabla() {
        let tbody = $("#tablaEmpleados tbody");
        tbody.empty();
        empleados.forEach((emp, i) => {
            tbody.append(`
                        <tr data-index="${i}">
                            <td>${emp.rolNombre}</td>
                            <td>${emp.nombre}</td>
                            <td>${emp.credencial}</td>
                            <td>${emp.telefono}</td>
                            <td>${emp.email}</td>
                            <td>${emp.direccion}</td>
                            <td>${emp.fechaNacimiento}</td>
                        </tr>
                    `);
        });

        // Solo agregar el evento click si estamos en modo normal
        if (modo === 'normal') {
            $("#tablaEmpleados tbody tr").click(function () {
                $("#tablaEmpleados tbody tr").removeClass("table-primary");
                $(this).addClass("table-primary");
                filaSeleccionada = $(this).data("index");

                // Llenar formulario
                let emp = empleados[filaSeleccionada];
                $("#idEmpleado").val(emp.idEmpleado);
                $("#idRol").val(emp.idRol);
                $("#rol").val(emp.idRol);
                $("#nombre").val(emp.nombre);
                $("#credencial").val(emp.credencial);
                $("#contrasena").val(emp.contrasena);
                $("#telefono").val(emp.telefono);
                $("#email").val(emp.email);
                $("#direccion").val(emp.direccion);
                $("#fechaNacimiento").val(emp.fechaNacimiento);
            });
        }
    }

    function cargarEmpleados() {
        $.ajax({
            url: "/api/Empleados",
            method: "GET",
            success: function (data) {
                empleados = data.map(emp => ({
                    idEmpleado: emp.idEmpleado,
                    idRol: emp.idRol,
                    rolNombre: emp.rol.trim(),
                    nombre: emp.nombre.trim(),
                    credencial: emp.credencial.trim(),
                    contrasena: emp.contraseña.trim(),
                    telefono: emp.telefono.trim(),
                    email: emp.email.trim(),
                    direccion: emp.direccion.trim(),
                    fechaNacimiento: emp.fechaNacimiento
                }));
                renderTabla();
            },
            error: function (err) {
                console.error("Error cargando empleados:", err);
            }
        });
    }

    function activarModoAgregar() {
        modo = 'agregar';
        $("#btnAgregar").text("Guardar").removeClass("btn-primary").addClass("btn-success");
        $("#btnEditar").text("Cancelar").removeClass("btn-warning").addClass("btn-secondary");
        $("#btnEliminar").prop("disabled", true);
        $("#tablaEmpleados").addClass("tabla-desactivada");
        $("#formEmpleado")[0].reset();
        filaSeleccionada = null;
        renderTabla(); // Re-render para quitar eventos de click
    }

    function activarModoEditar() {
        if (filaSeleccionada === null) {
            alert("Seleccione un empleado para editar");
            return;
        }
        modo = 'editar';
        $("#btnAgregar").text("Guardar Cambios").removeClass("btn-primary").addClass("btn-success");
        $("#btnEditar").text("Cancelar").removeClass("btn-warning").addClass("btn-secondary");
        $("#btnEliminar").prop("disabled", true);
        $("#tablaEmpleados").addClass("tabla-desactivada");
        renderTabla(); // Re-render para quitar eventos de click
    }

    function desactivarModo() {
        modo = 'normal';
        $("#btnAgregar").text("Agregar").removeClass("btn-success").addClass("btn-primary");
        $("#btnEditar").text("Editar").removeClass("btn-secondary").addClass("btn-warning");
        $("#btnEliminar").prop("disabled", false);
        $("#tablaEmpleados").removeClass("tabla-desactivada");
        $("#formEmpleado")[0].reset();
        filaSeleccionada = null;
        renderTabla(); // Re-render para restaurar eventos
    }

    $(document).ready(function () {
        cargarEmpleados();

        // Botón Agregar / Guardar
        $("#btnAgregar").click(function () {
            if (modo === 'agregar') {
                // Guardar nuevo empleado
                if (!$("#formEmpleado")[0].checkValidity()) {
                    $("#formEmpleado")[0].reportValidity();
                    return;
                }
                let nuevo = {
                    idRol: Number($("#rol").val()),
                    nombre: $("#nombre").val(),
                    credencial: $("#credencial").val(),
                    contraseña: $("#contrasena").val(),
                    telefono: $("#telefono").val(),
                    email: $("#email").val(),
                    direccion: $("#direccion").val(),
                    fechaNacimiento: $("#fechaNacimiento").val(),
                    estado: true
                };
                $.ajax({
                    url: "/api/Empleados",
                    method: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(nuevo),
                    success: function () {
                        alert("Empleado agregado");
                        cargarEmpleados();
                        desactivarModo();
                    },
                    error: function (err) { console.error(err); }
                });
            } else if (modo === 'editar') {
                // Guardar cambios en edición
                if (!$("#formEmpleado")[0].checkValidity()) {
                    $("#formEmpleado")[0].reportValidity();
                    return;
                }
                let actualizar = {
                    idEmpleado: Number($("#idEmpleado").val()),
                    idRol: Number($("#rol").val()),
                    nombre: $("#nombre").val(),
                    credencial: $("#credencial").val(),
                    contraseña: $("#contrasena").val(),
                    telefono: $("#telefono").val(),
                    email: $("#email").val(),
                    direccion: $("#direccion").val(),
                    fechaNacimiento: $("#fechaNacimiento").val(),
                    estado: true
                };
                $.ajax({
                    url: `/api/Empleados/${actualizar.idEmpleado}`,
                    method: "PUT",
                    contentType: "application/json",
                    data: JSON.stringify(actualizar),
                    success: function () {
                        alert("Empleado actualizado");
                        cargarEmpleados();
                        desactivarModo();
                    },
                    error: function (err) { console.error(err); }
                });
            } else {
                // Inicia modo agregar
                activarModoAgregar();
            }
        });

        // Botón Editar / Cancelar
        $("#btnEditar").click(function () {
            if (modo === 'agregar' || modo === 'editar') {
                // Cancelar
                desactivarModo();
            } else {
                // Inicia modo editar
                activarModoEditar();
            }
        });

        // Eliminar empleado
        $("#btnEliminar").click(function () {
            if (modo !== 'normal') return; // Deshabilitado en modos agregar/editar
            if (filaSeleccionada === null) {
                alert("Seleccione un empleado para eliminar");
                return;
            }
            let id = Number($("#idEmpleado").val());
            if (confirm("¿Desea eliminar este empleado?")) {
                $.ajax({
                    url: `/api/Empleados/${id}`,
                    method: "DELETE",
                    success: function () {
                        alert("Empleado eliminado");
                        cargarEmpleados();
                        $("#formEmpleado")[0].reset();
                        filaSeleccionada = null;
                    },
                    error: function (err) { console.error(err); }
                });
            }
        });
    });
})();