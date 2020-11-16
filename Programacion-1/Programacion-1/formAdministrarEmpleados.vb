﻿Public Class formAdministrarEmpleados
    'Realiza la conexion a la base solo para este formulario
    Dim objConexion As New Conexion
    Dim dataTable As New DataTable
    Dim accion As String = "nuevo"
    Dim comandosql = ""

    Dim mensajeenmentana = "Registro de Empleados"
    Dim Nombretabladebusqueda = "Empleados"
    Dim buscarpor1 = "NombreCompleto"
    Dim buscarpor2 = "DUI"
    Dim idTabla = "IdEmpleado"
    Dim comandoinsertar = Nombretabladebusqueda + " (NombreCompleto,DUI,Telefono,Correo,Direccion)" 'campos de la tabla en orden menos id
    Dim comandoactualizar = Nombretabladebusqueda

    'accion al cargar el formulario
    Private Sub formAdministrarEmpleados_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtid.Visible = False
        Label7.Visible = False
        obtenerdatos()
    End Sub


    'obtiene los datos y los recarga en el gridview llamado grid
    Sub obtenerdatos()
        Try
            'la palabra Empleados es la palabra que envia la peticion de la tabla que quiere
            'la palabra datos tabla es la que recibe los resultados de la tabla
            'llenar los datos del grid
            grid.DataSource = objConexion.obtenerDatos().Tables("Empleados").DefaultView
            grid.Columns(0).Visible = False
        Catch ex As Exception
            'Mensaje si no hay datos que mostra
            MsgBox("No hay datos en la Base de Datos " & ex.Message)
        End Try
    End Sub


    'Boton primero
    Private Sub btnnuevoyaceptar_Click(sender As Object, e As EventArgs) Handles btnnuevoyaceptar.Click
        If btnnuevoyaceptar.Text = "Nuevo" Then 'Nuevo
            btnnuevoyaceptar.Text = "Aceptar"
            btnmodificarycancelar.Text = "Cancelar"
            accion = "nuevo"
            btneliminar.Enabled = False
            limpiar()


            'si el boton dice aceptar, significa que esta aceptando el nuevo registro y lo envia a la base
        ElseIf btnnuevoyaceptar.Text = "Aceptar" Then
            comandosql = comandoinsertar

            Dim msg = objConexion.mantenimientoEmpleados(New String() {
              "",                 'dato(0) para el id, incrementa automaticamente no necesita enviar nada 
              txtnombre.Text,     'dato(1)
              txtdui.Text,        'dato(2)
              txttelefono.Text,   'dato(3)
              txtcorreo.Text,     'dato(4)
              txtdireccion.Text}, 'dato(5)
            accion, comandosql, idTabla) 'accion que se desea realizar en el case
            btnnuevoyaceptar.Text = "Nuevo"
            btnmodificarycancelar.Text = "Modificar"
            obtenerdatos()
            limpiar()
            MessageBox.Show(msg, mensajeenmentana, MessageBoxButtons.OK, MessageBoxIcon.Information)
            btneliminar.Enabled = True


        Else 'si el boton dice Guardar, significa que esta haciendo un cambio de un dato
            comandosql = comandoactualizar
            Dim msg = objConexion.mantenimientoEmpleados(New String() {
              txtid.Text,      'dato(0) si se envia el id aqui porque es el que identifica el registro, update from id = x
              txtnombre.Text,  'dato(1)
              txtdui.Text,     'dato(2)
              txttelefono.Text,'dato(3)
              txtcorreo.Text,   'dato(4)
              txtdireccion.Text}, 'dato(5)
              accion, comandosql, idTabla)

            obtenerdatos()
            MessageBox.Show(msg, mensajeenmentana, MessageBoxButtons.OK, MessageBoxIcon.Information)
            limpiar()
            btnnuevoyaceptar.Text = "Nuevo"
            btnmodificarycancelar.Text = "Modificar"
            btneliminar.Enabled = True
        End If
    End Sub



    Private Sub btnmodificarycancelar_Click(sender As Object, e As EventArgs) Handles btnmodificarycancelar.Click
        If btnmodificarycancelar.Text = "Modificar" Then 'Nuevo
            btnnuevoyaceptar.Text = "Guardar"
            btnmodificarycancelar.Text = "Cancelar"
            btneliminar.Enabled = False
            accion = "modificar"
        Else 'Guardar
            btnnuevoyaceptar.Text = "Nuevo"
            btnmodificarycancelar.Text = "Modificar"
            obtenerdatos()
            btneliminar.Enabled = True
        End If
    End Sub





    Private Sub btneliminar_Click(sender As Object, e As EventArgs) Handles btneliminar.Click
        If txtid.Text <> "" Then
            If (MessageBox.Show("Esta seguro de borrar a " + txtnombre.Text, mensajeenmentana,
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes) Then
                comandosql = Nombretabladebusqueda
                Dim msg = objConexion.mantenimientoEmpleados(New String() {txtid.Text}, "eliminar", comandosql, idTabla)
                If msg = "Error en el proceso" Then
                    MessageBox.Show("No se pudo eliminar este registro, porque hay registros que dependen de el", mensajeenmentana, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        Else MessageBox.Show("Debe selecionar un registro para eliminar", mensajeenmentana)
        End If
        limpiar()
        obtenerdatos()
    End Sub


    'filtro del datagridview
    Private Sub txtfiltro_KeyUp(sender As Object, e As KeyEventArgs) Handles txtfiltro.KeyUp
        filtro(txtfiltro.Text)
    End Sub
    Private Sub filtro(ByVal valor As String)
        Dim bs As New BindingSource()
        bs.DataSource = grid.DataSource
        bs.Filter = buscarpor1 + " like '%" & valor & "%' or " + buscarpor2 + " like '%" & valor & "%'"
        grid.DataSource = bs
    End Sub


    'pasar datos del grid al dar click hacia los txt
    Private Sub grid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles grid.CellClick
        If btnnuevoyaceptar.Text <> "Aceptar" Then


            If grid.Rows.Count > 0 Then
                Dim i As Integer
                i = grid.CurrentRow.Index
                txtid.Text = grid.Item(0, i).Value()
                txtnombre.Text = grid.Item(1, i).Value()
                txtdui.Text = grid.Item(2, i).Value()
                txttelefono.Text = grid.Item(3, i).Value()
                txtcorreo.Text = grid.Item(4, i).Value()
                txtdireccion.Text = grid.Item(5, i).Value()
            End If

        End If
    End Sub


    'limpia los campos
    Private Sub limpiar()
        txtid.Text = ""
        txtnombre.Text = ""
        txtdui.Text = ""
        txttelefono.Text = ""
        txtcorreo.Text = ""
        txtdireccion.Text = ""
    End Sub

End Class