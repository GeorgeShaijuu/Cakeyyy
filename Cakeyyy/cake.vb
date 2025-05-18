Imports MySql.Data.MySqlClient

Public Class cake
    Dim connectionString As String = "server=localhost;userid=root;password=admin;database=cakey;"
    Dim selectedCakeId As Integer = -1

    Private Sub cake_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadCakeData()
    End Sub

    ' ✅ Load data into DataGridView
    Private Sub LoadCakeData()
        Using conn As New MySqlConnection(connectionString)
            Try
                conn.Open()
                Dim query As String = "SELECT * FROM cake"
                Dim adapter As New MySqlDataAdapter(query, conn)
                Dim table As New DataTable()
                adapter.Fill(table)
                Guna2DataGridView1.DataSource = table
            Catch ex As Exception
                MessageBox.Show("Error loading cakes: " & ex.Message)
            End Try
        End Using
    End Sub

    ' ✅ Save or Update Cake
    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Dim size As String = Guna2TextBox1.Text.Trim()
        Dim priceText As String = Guna2TextBox4.Text.Trim()
        Dim flavour As String = Guna2TextBox2.Text.Trim()
        Dim stockText As String = Guna2TextBox5.Text.Trim()
        Dim preference As String = Guna2TextBox3.Text.Trim()

        If size = "" Or priceText = "" Or flavour = "" Or stockText = "" Or preference = "" Then
            MessageBox.Show("All fields are required.")
            Return
        End If

        If Not IsNumeric(priceText) Or Not IsNumeric(stockText) Then
            MessageBox.Show("Price and Stock must be numeric.")
            Return
        End If

        Dim price As Integer = Convert.ToInt32(priceText)
        Dim stock As Integer = Convert.ToInt32(stockText)

        Using conn As New MySqlConnection(connectionString)
            Try
                conn.Open()
                Dim query As String

                If selectedCakeId = -1 Then
                    ' Insert
                    query = "INSERT INTO cake (size, price, flavour, stock, preference) VALUES (@size, @price, @flavour, @stock, @preference)"
                Else
                    ' Update
                    query = "UPDATE cake SET size=@size, price=@price, flavour=@flavour, stock=@stock, preference=@preference WHERE cake_id=@id"
                End If

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@size", size)
                    cmd.Parameters.AddWithValue("@price", price)
                    cmd.Parameters.AddWithValue("@flavour", flavour)
                    cmd.Parameters.AddWithValue("@stock", stock)
                    cmd.Parameters.AddWithValue("@preference", preference)

                    If selectedCakeId <> -1 Then
                        cmd.Parameters.AddWithValue("@id", selectedCakeId)
                    End If

                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Cake saved successfully.")
                End Using

                ClearForm()
                LoadCakeData()

            Catch ex As Exception
                MessageBox.Show("Error saving cake: " & ex.Message)
            End Try
        End Using
    End Sub

    ' ✅ Clear the form
    Private Sub ClearForm()
        Guna2TextBox1.Clear()
        Guna2TextBox2.Clear()
        Guna2TextBox3.Clear()
        Guna2TextBox4.Clear()
        Guna2TextBox5.Clear()
        selectedCakeId = -1
    End Sub

    ' ✅ Fill fields when a row is selected
    Private Sub Guna2DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = Guna2DataGridView1.Rows(e.RowIndex)
            selectedCakeId = Convert.ToInt32(row.Cells("cake_id").Value)
            Guna2TextBox1.Text = row.Cells("size").Value.ToString()
            Guna2TextBox4.Text = row.Cells("price").Value.ToString()
            Guna2TextBox2.Text = row.Cells("flavour").Value.ToString()
            Guna2TextBox5.Text = row.Cells("stock").Value.ToString()
            Guna2TextBox3.Text = row.Cells("preference").Value.ToString()
        End If
    End Sub

    ' ✅ Delete cake
    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        If selectedCakeId = -1 Then
            MessageBox.Show("Please select a cake to delete.")
            Return
        End If

        Dim confirmResult As DialogResult = MessageBox.Show("Are you sure you want to delete this cake?", "Confirm Delete", MessageBoxButtons.YesNo)
        If confirmResult = DialogResult.No Then Exit Sub

        Using conn As New MySqlConnection(connectionString)
            Try
                conn.Open()
                Dim query As String = "DELETE FROM cake WHERE cake_id = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", selectedCakeId)
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Cake deleted successfully.")
                End Using

                ClearForm()
                LoadCakeData()

            Catch ex As Exception
                MessageBox.Show("Error deleting cake: " & ex.Message)
            End Try
        End Using
    End Sub

End Class
