Imports MySql.Data.MySqlClient

Public Class cart
    Dim connectionString As String = "server=localhost;userid=root;password=admin;database=cakey;"
    Dim selectedCartId As Integer = -1

    Private Sub cart_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadComboBoxes()
        LoadCartData()

        ' Ensure only one checkbox is selected
        AddHandler Guna2CheckBox1.CheckedChanged, AddressOf CheckBoxHandler
        AddHandler Guna2CheckBox2.CheckedChanged, AddressOf CheckBoxHandler
    End Sub

    ' ✅ Load ComboBoxes with names and IDs
    Private Sub LoadComboBoxes()
        ' Load customer names
        Using conn As New MySqlConnection(connectionString)
            Try
                conn.Open()
                Dim custQuery As String = "SELECT cid, c_name FROM customer"
                Using cmd As New MySqlCommand(custQuery, conn)
                    Using reader = cmd.ExecuteReader()
                        Guna2ComboBox1.Items.Clear()
                        While reader.Read()
                            Guna2ComboBox1.Items.Add(New With {.Text = $"{reader("cid")} - {reader("c_name")}", .Value = reader("cid")})
                        End While
                    End Using
                End Using

                ' Load cake flavours
                Dim cakeQuery As String = "SELECT cake_id, flavour FROM cake"
                Using cmd As New MySqlCommand(cakeQuery, conn)
                    Using reader = cmd.ExecuteReader()
                        Guna2ComboBox2.Items.Clear()
                        While reader.Read()
                            Guna2ComboBox2.Items.Add(New With {.Text = $"{reader("cake_id")} - {reader("flavour")}", .Value = reader("cake_id")})
                        End While
                    End Using
                End Using

            Catch ex As Exception
                MessageBox.Show("Error loading combo boxes: " & ex.Message)
            End Try
        End Using

        Guna2ComboBox1.DisplayMember = "Text"
        Guna2ComboBox1.ValueMember = "Value"

        Guna2ComboBox2.DisplayMember = "Text"
        Guna2ComboBox2.ValueMember = "Value"
    End Sub

    ' ✅ Handle single checkbox selection
    Private Sub CheckBoxHandler(sender As Object, e As EventArgs)
        If sender Is Guna2CheckBox1 AndAlso Guna2CheckBox1.Checked Then
            Guna2CheckBox2.Checked = False
        ElseIf sender Is Guna2CheckBox2 AndAlso Guna2CheckBox2.Checked Then
            Guna2CheckBox1.Checked = False
        End If
    End Sub

    ' ✅ Load DataGridView with cart data
    Private Sub LoadCartData()
        Using conn As New MySqlConnection(connectionString)
            Try
                conn.Open()
                Dim query As String = "SELECT * FROM cart"
                Dim adapter As New MySqlDataAdapter(query, conn)
                Dim table As New DataTable()
                adapter.Fill(table)
                Guna2DataGridView1.DataSource = table
            Catch ex As Exception
                MessageBox.Show("Error loading cart data: " & ex.Message)
            End Try
        End Using
    End Sub

    ' ✅ Save or update cart data
    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        If Guna2ComboBox1.SelectedItem Is Nothing OrElse Guna2ComboBox2.SelectedItem Is Nothing Then
            MessageBox.Show("Please select customer and cake.")
            Return
        End If

        If Not Guna2CheckBox1.Checked And Not Guna2CheckBox2.Checked Then
            MessageBox.Show("Select takeaway option.")
            Return
        End If

        Dim cid As Integer = CType(Guna2ComboBox1.SelectedItem, Object).Value
        Dim cake_id As Integer = CType(Guna2ComboBox2.SelectedItem, Object).Value
        Dim takeaway As String = If(Guna2CheckBox1.Checked, "False", "True")
        Dim cartDate As String = Guna2TextBox3.Text.Trim()

        If cartDate = "" Then
            MessageBox.Show("Enter the date.")
            Return
        End If

        Using conn As New MySqlConnection(connectionString)
            Try
                conn.Open()
                Dim query As String

                If selectedCartId = -1 Then
                    query = "INSERT INTO cart (cake_id, cid, date, takeaway) VALUES (@cake_id, @cid, @date, @takeaway)"
                Else
                    query = "UPDATE cart SET cake_id=@cake_id, cid=@cid, date=@date, takeaway=@takeaway WHERE cart_id=@cart_id"
                End If

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@cake_id", cake_id)
                    cmd.Parameters.AddWithValue("@cid", cid)
                    cmd.Parameters.AddWithValue("@date", cartDate)
                    cmd.Parameters.AddWithValue("@takeaway", takeaway)
                    If selectedCartId <> -1 Then cmd.Parameters.AddWithValue("@cart_id", selectedCartId)
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Cart saved successfully.")
                End Using

                ClearForm()
                LoadCartData()
            Catch ex As Exception
                MessageBox.Show("Error saving cart: " & ex.Message)
            End Try
        End Using
    End Sub

    ' ✅ Select data from grid to edit
    Private Sub Guna2DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = Guna2DataGridView1.Rows(e.RowIndex)
            selectedCartId = Convert.ToInt32(row.Cells("cart_id").Value)
            Guna2TextBox3.Text = row.Cells("date").Value.ToString()

            ' Set selected combo values
            For Each item In Guna2ComboBox1.Items
                If item.Value = Convert.ToInt32(row.Cells("cid").Value) Then
                    Guna2ComboBox1.SelectedItem = item
                    Exit For
                End If
            Next

            For Each item In Guna2ComboBox2.Items
                If item.Value = Convert.ToInt32(row.Cells("cake_id").Value) Then
                    Guna2ComboBox2.SelectedItem = item
                    Exit For
                End If
            Next

            ' Set checkbox
            If row.Cells("takeaway").Value.ToString().ToLower() = "true" Then
                Guna2CheckBox2.Checked = True
            Else
                Guna2CheckBox1.Checked = True
            End If
        End If
    End Sub

    ' ✅ Clear form
    Private Sub ClearForm()
        Guna2ComboBox1.SelectedIndex = -1
        Guna2ComboBox2.SelectedIndex = -1
        Guna2TextBox3.Clear()
        Guna2CheckBox1.Checked = False
        Guna2CheckBox2.Checked = False
        selectedCartId = -1
    End Sub

    ' ✅ Delete cart
    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        If selectedCartId = -1 Then
            MessageBox.Show("Please select a cart item to delete.")
            Return
        End If

        If MessageBox.Show("Are you sure to delete this cart item?", "Confirm Delete", MessageBoxButtons.YesNo) = DialogResult.No Then Return

        Using conn As New MySqlConnection(connectionString)
            Try
                conn.Open()
                Dim query As String = "DELETE FROM cart WHERE cart_id = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", selectedCartId)
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Cart item deleted.")
                End Using

                ClearForm()
                LoadCartData()
            Catch ex As Exception
                MessageBox.Show("Error deleting cart: " & ex.Message)
            End Try
        End Using
    End Sub
End Class
