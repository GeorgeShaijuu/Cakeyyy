Imports MySql.Data.MySqlClient

Public Class Form1
    Dim connectionString As String = "server=localhost;userid=root;password=admin;database=cakey;"

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Dim username As String = Guna2TextBox1.Text.Trim()
        Dim password As String = Guna2TextBox2.Text.Trim()

        If username = "" Or password = "" Then
            MessageBox.Show("Please enter both username and password")
            Return
        End If

        Using conn As New MySqlConnection(connectionString)
            Try
                conn.Open()
                Dim query As String = "SELECT * FROM admin WHERE username = @username AND password = @password"
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@username", username)
                cmd.Parameters.AddWithValue("@password", password)

                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    MessageBox.Show("Login successful!")
                    Dim usergnt As New usermgnt
                    usergnt.Show()
                    Me.Hide()
                Else
                    MessageBox.Show("Invalid username or password!")
                End If
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            End Try
        End Using
    End Sub
End Class
