Imports MySql.Data.MySqlClient

Public Class usermgnt
    Dim connectionString As String = "server=localhost;userid=root;password=admin;database=cakey;"
    Dim selectedCustomerId As Integer = -1

    Private Sub usermgnt_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadCustomerData()
    End Sub

    ' ✅ Load all customer data into DataGridView
    Private Sub LoadCustomerData()
        Using conn As New MySqlConnection(connectionString)
            Try
                conn.Open()
                Dim query As String = "SELECT * FROM customer"
                Dim adapter As New MySqlDataAdapter(query, conn)
                Dim table As New DataTable()
                adapter.Fill(table)
                Guna2DataGridView1.DataSource = table
            Catch ex As Exception
                MessageBox.Show("Error loading data: " & ex.Message)
            End Try
        End Using
    End Sub

    ' ✅ Button Click to Insert or Update
    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Dim name As String = Guna2TextBox1.Text.Trim()
        Dim phone As String = Guna2TextBox2.Text.Trim()
        Dim address As String = Guna2TextBox3.Text.Trim()

        If name = "" Or phone = "" Or address = "" Then
            MessageBox.Show("All fields are required!")
            Return
        End If

        If Not IsNumeric(phone) Or phone.Length <> 10 Then
            MessageBox.Show("Phone number must be 10 digits only.")
            Return
        End If

        Using conn As New MySqlConnection(connectionString)
            Try
                conn.Open()

                Dim query As String
                If selectedCustomerId = -1 Then
                    ' Insert new
                    query = "INSERT INTO customer (c_name, phone_no, address) VALUES (@name, @phone, @address)"
                Else
                    ' Update existing
                    query = "UPDATE customer SET c_name = @name, phone_no = @phone, address = @address WHERE cid = @id"
                End If

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@name", name)
                    cmd.Parameters.AddWithValue("@phone", phone)
                    cmd.Parameters.AddWithValue("@address", address)

                    If selectedCustomerId <> -1 Then
                        cmd.Parameters.AddWithValue("@id", selectedCustomerId)
                    End If

                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Data saved successfully.")
                End Using

                ' Reset form
                ClearForm()
                LoadCustomerData()

            Catch ex As Exception
                MessageBox.Show("Error saving data: " & ex.Message)
            End Try
        End Using
    End Sub

    ' ✅ Clear Textboxes and Reset ID
    Private Sub ClearForm()
        Guna2TextBox1.Text = ""
        Guna2TextBox2.Text = ""
        Guna2TextBox3.Text = ""
        selectedCustomerId = -1
    End Sub

    ' ✅ On row click, fill the textboxes
    Private Sub Guna2DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = Guna2DataGridView1.Rows(e.RowIndex)
            selectedCustomerId = Convert.ToInt32(row.Cells("cid").Value)
            Guna2TextBox1.Text = row.Cells("c_name").Value.ToString()
            Guna2TextBox2.Text = row.Cells("phone_no").Value.ToString()
            Guna2TextBox3.Text = row.Cells("address").Value.ToString()
        End If
    End Sub
    Private Sub Guna2PictureBox2_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox2.Click
        ' Create instance of the cake form
        Dim cakeForm As New cake()

        ' Set the form's start position to manual
        cakeForm.StartPosition = FormStartPosition.Manual

        ' Position it where Guna2PictureBox2 is, relative to the screen
        cakeForm.Location = Me.PointToScreen(Guna2CustomGradientPanel2.Location)

        ' Hide the custom gradient panel
        Guna2CustomGradientPanel2.Visible = False

        ' Show the form
        cakeForm.Show()
    End Sub

    Private Sub Guna2PictureBox1_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox1.Click
        Guna2CustomGradientPanel2.Visible = True
    End Sub

    Private Sub Guna2PictureBox3_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox3.Click
        ' Create instance of the cake form
        Dim cakeForm As New cart()

        ' Set the form's start position to manual
        cakeForm.StartPosition = FormStartPosition.Manual

        ' Position it where Guna2PictureBox2 is, relative to the screen
        cakeForm.Location = Me.PointToScreen(Guna2CustomGradientPanel2.Location)

        ' Hide the custom gradient panel
        Guna2CustomGradientPanel2.Visible = False

        ' Show the form
        cakeForm.Show()
    End Sub

    Private Sub Guna2PictureBox4_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox4.Click
        ' Create instance of the cake form
        Dim cakeForm As New payment()

        ' Set the form's start position to manual
        cakeForm.StartPosition = FormStartPosition.Manual

        ' Position it where Guna2PictureBox2 is, relative to the screen
        cakeForm.Location = Me.PointToScreen(Guna2CustomGradientPanel2.Location)

        ' Hide the custom gradient panel
        Guna2CustomGradientPanel2.Visible = False

        ' Show the form
        cakeForm.Show()
    End Sub
    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        If selectedCustomerId = -1 Then
            MessageBox.Show("Please select a customer to delete.")
            Return
        End If

        Dim confirmResult As DialogResult = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", MessageBoxButtons.YesNo)
        If confirmResult = DialogResult.No Then Exit Sub

        Using conn As New MySqlConnection(connectionString)
            Try
                conn.Open()
                Dim query As String = "DELETE FROM customer WHERE cid = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", selectedCustomerId)
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Customer deleted successfully.")
                End Using

                ' Refresh
                ClearForm()
                LoadCustomerData()

            Catch ex As Exception
                MessageBox.Show("Error deleting customer: " & ex.Message)
            End Try
        End Using
    End Sub

End Class
