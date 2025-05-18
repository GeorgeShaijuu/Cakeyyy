Imports MySql.Data.MySqlClient
Public Class payment
    Dim con As New MySqlConnection("server=localhost;userid=root;password=admin;database=cakey")
    Dim cmd As MySqlCommand
    Dim dt As DataTable
    Dim selectedPaymentId As Integer = -1
    Dim remainingSeconds As Integer = 120

    Private Sub payment_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadCartCombo()
        LoadMethods()
        LoadPayments()
        SetUPIVisibility(False)
    End Sub

    Private Sub LoadCartCombo()
        Guna2ComboBox1.Items.Clear()
        con.Open()
        Dim da As New MySqlDataAdapter("SELECT c.cart_id, cu.c_name FROM cart c JOIN customer cu ON c.cid = cu.cid", con)
        dt = New DataTable()
        da.Fill(dt)
        For Each row As DataRow In dt.Rows
            Guna2ComboBox1.Items.Add($"{row("cart_id")} - {row("c_name")}")
        Next
        con.Close()
    End Sub

    Private Sub LoadMethods()
        Guna2ComboBox2.Items.Clear()
        Guna2ComboBox2.Items.Add("UPI")
        Guna2ComboBox2.Items.Add("Card")
        Guna2ComboBox2.Items.Add("Cash")
    End Sub

    Private Sub LoadPayments()
        con.Open()
        Dim da As New MySqlDataAdapter("SELECT * FROM payment", con)
        Dim dtGrid As New DataTable()
        da.Fill(dtGrid)
        Guna2DataGridView1.DataSource = dtGrid
        con.Close()
    End Sub

    Private Sub SetUPIVisibility(visible As Boolean)
        Label5.Visible = visible
        Label6.Visible = visible
        Label7.Visible = visible
        Guna2PictureBox1.Visible = visible
        If Not visible Then
            Timer1.Stop()
            Label7.Text = ""
        End If
    End Sub

    Private Sub Guna2ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox2.SelectedIndexChanged
        If Guna2ComboBox2.SelectedItem = "UPI" Then
            SetUPIVisibility(True)
            remainingSeconds = 120
            Timer1.Start()
        Else
            SetUPIVisibility(False)
        End If

        If Guna2ComboBox2.SelectedItem = "Card" Then
            Dim cardInfo = InputBox("Enter Card Details (Number, CVV, MM/YY, Name):", "Card Payment")
            MessageBox.Show("Details Entered: " & cardInfo)
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        remainingSeconds -= 1
        Dim mins As Integer = remainingSeconds \ 60
        Dim secs As Integer = remainingSeconds Mod 60
        Label7.Text = $"{mins:D2}:{secs:D2}"
        If remainingSeconds <= 0 Then
            Timer1.Stop()
            MessageBox.Show("Time Limit Exhausted")
            SetUPIVisibility(False)
        End If
    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        If Guna2ComboBox1.SelectedIndex = -1 Or Guna2ComboBox2.SelectedIndex = -1 Or Guna2TextBox3.Text = "" Then
            MessageBox.Show("Fill all fields.")
            Return
        End If

        Dim cartParts = Guna2ComboBox1.SelectedItem.ToString().Split(" - ")
        Dim cart_id As Integer = Integer.Parse(cartParts(0))
        Dim cidQuery = "SELECT cid, cake_id FROM cart WHERE cart_id = @cart_id"
        con.Open()
        cmd = New MySqlCommand(cidQuery, con)
        cmd.Parameters.AddWithValue("@cart_id", cart_id)
        Dim reader = cmd.ExecuteReader()
        reader.Read()
        Dim cid = reader("cid")
        Dim cake_id = reader("cake_id")
        con.Close()

        Dim method = Guna2ComboBox2.SelectedItem.ToString()
        Dim amount = Integer.Parse(Guna2TextBox3.Text)
        Dim pay_date = Date.Now.ToString("yyyy-MM-dd")

        con.Open()
        If selectedPaymentId = -1 Then
            cmd = New MySqlCommand("INSERT INTO payment(cart_id, cake_id, cid, method, amount, date) VALUES(@cart_id, @cake_id, @cid, @method, @amount, @date)", con)
        Else
            cmd = New MySqlCommand("UPDATE payment SET cart_id=@cart_id, cake_id=@cake_id, cid=@cid, method=@method, amount=@amount, date=@date WHERE pay_id=@pay_id", con)
            cmd.Parameters.AddWithValue("@pay_id", selectedPaymentId)
        End If

        cmd.Parameters.AddWithValue("@cart_id", cart_id)
        cmd.Parameters.AddWithValue("@cake_id", cake_id)
        cmd.Parameters.AddWithValue("@cid", cid)
        cmd.Parameters.AddWithValue("@method", method)
        cmd.Parameters.AddWithValue("@amount", amount)
        cmd.Parameters.AddWithValue("@date", pay_date)
        cmd.ExecuteNonQuery()
        con.Close()

        LoadPayments()
        MessageBox.Show("Saved successfully!")
        ClearForm()
    End Sub

    Private Sub Guna2DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim row = Guna2DataGridView1.Rows(e.RowIndex)
            selectedPaymentId = row.Cells("pay_id").Value
            Guna2ComboBox1.Text = row.Cells("cart_id").Value.ToString()
            Guna2ComboBox2.Text = row.Cells("method").Value.ToString()
            Guna2TextBox3.Text = row.Cells("amount").Value.ToString()
        End If
    End Sub


    Private Sub ClearForm()
        Guna2ComboBox1.SelectedIndex = -1
        Guna2ComboBox2.SelectedIndex = -1
        Guna2TextBox3.Clear()
        selectedPaymentId = -1
        SetUPIVisibility(False)
    End Sub

    Private Sub Guna2GradientButton3_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton3.Click
        If Guna2DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a payment entry from the table.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Dim selectedRow As DataGridViewRow = Guna2DataGridView1.SelectedRows(0)
        Dim pay_id As Integer = Convert.ToInt32(selectedRow.Cells("pay_id").Value)

        Try
            Dim query As String = "
            SELECT 
                p.pay_id,
                c.c_name AS CustomerName,
                ck.flavour AS CakeFlavour,
                ck.size AS CakeSize,
                ck.price AS CakePrice,
                crt.takeaway,
                p.method AS PaymentMethod,
                p.date AS PaymentDate,
                p.amount AS TotalAmount
            FROM 
                payment p
            JOIN 
                customer c ON p.cid = c.cid
            JOIN 
                cart crt ON p.cart_id = crt.cart_id
            JOIN 
                cake ck ON p.cake_id = ck.cake_id
            WHERE 
                p.pay_id = @pay_id
        "

            Using con As New MySqlConnection("server=localhost;userid=root;password=admin;database=cakey")
                con.Open()
                Using cmd As New MySqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@pay_id", pay_id)
                    Dim reader As MySqlDataReader = cmd.ExecuteReader()

                    If reader.Read() Then
                        Dim bill As String = ""
                        bill &= "-------- Cakelicious Bill --------" & Environment.NewLine
                        bill &= "Bill ID: " & reader("pay_id") & Environment.NewLine
                        bill &= "Customer Name: " & reader("CustomerName") & Environment.NewLine
                        bill &= "Cake: " & reader("CakeFlavour") & " (" & reader("CakeSize") & ")" & Environment.NewLine
                        bill &= "Cake Price: ₹" & reader("CakePrice") & Environment.NewLine
                        bill &= "Takeaway: " & If(reader("takeaway").ToString.ToLower = "true", "Yes", "No") & Environment.NewLine
                        bill &= "Payment Method: " & reader("PaymentMethod") & Environment.NewLine
                        bill &= "Payment Date: " & reader("PaymentDate") & Environment.NewLine
                        bill &= "Total Paid: ₹" & reader("TotalAmount") & Environment.NewLine
                        bill &= "----------------------------------"

                        MessageBox.Show(bill, "Generated Bill", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBox.Show("Bill data not found.")
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error generating bill: " & ex.Message)
        End Try
    End Sub

End Class
