Public Class usermgnt
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
End Class