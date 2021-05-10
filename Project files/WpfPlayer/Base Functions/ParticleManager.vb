Imports System.Windows.Media.Effects

Public Class ParticleManager
    Public Property ParticleHost As Canvas
    Public Property Host As Window
    Private timer As System.Windows.Threading.DispatcherTimer = New System.Windows.Threading.DispatcherTimer()
    'Private timer As Forms.Timer = New Forms.Timer()
    Private random As Random = New Random(DateTime.Now.Millisecond)
    ' Some general values
    Public MaxSize As Double = 150
    Public NumberOfParticles As Double = 25
    Public VerticalVelocity As Double = 0.4
    Public HorizontalVelocity As Double = -2.2
    Public Color As Brush = Brushes.White.Clone
    Public Blurry As Boolean = True
    Public Sub New(ParticlesHost As Canvas, Owner As Window, Optional ParticleMaxSize As Double = 150, Optional Count As Double = 25, Optional XVelocity As Double = 0.4, Optional YVelocity As Double = -2.2)
        Host = Owner
        ParticleHost = ParticlesHost
        MaxSize = ParticleMaxSize
        NumberOfParticles = Count
        VerticalVelocity = XVelocity
        HorizontalVelocity = YVelocity
        For i As Integer = 0 To NumberOfParticles - 1
            CreateParticle()
        Next

        timer.Interval = TimeSpan.FromMilliseconds(33.33)
        'timer.Interval = 33
        AddHandler timer.Tick, AddressOf timer_Tick
        timer.Start()
    End Sub
    Public Sub Disable()
        ParticleHost.Children.Clear()
        timer.Stop()
    End Sub
    Public Sub Enable()
        For i As Integer = 0 To NumberOfParticles - 1
            CreateParticle()
        Next
        timer.Start()
    End Sub
    Public Sub ApplyColor()
        For Each ellipse As Ellipse In ParticleHost.Children
            ' Brush (White)
            Dim brush = Color
            ' Opacity (0.2 <= 1)
            brush.Opacity = 0.2 + random.NextDouble() * 0.8
            TryCast(ellipse.Tag, Particle).Brush = brush
        Next
    End Sub
    Public Sub OnBlurApply()
        If Blurry = False Then
            For Each ellipse As Ellipse In ParticleHost.Children
                TryCast(ellipse.Tag, Particle).Blur = Nothing
                ellipse.Effect = Nothing
            Next
        Else
            For Each ellipse As Ellipse In ParticleHost.Children
                ' Blur effect
                Dim blur = New BlurEffect()
                blur.RenderingBias = RenderingBias.Performance
                ' Radius (1 <= 40)
                blur.Radius = 1 + random.NextDouble() * 39
                TryCast(ellipse.Tag, Particle).Blur = blur
                ellipse.Effect = blur
            Next
        End If
    End Sub
    Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        ' I control "particle" from their ellipse representation
        For Each ellipse As Ellipse In ParticleHost.Children
            Dim p = TryCast(ellipse.Tag, Particle)
            Dim t = TryCast(ellipse.RenderTransform, TranslateTransform)
            ' Update location
            t.X += p.Velocity.X
            t.Y -= p.Velocity.Y
            ' Check if the particle Is too high
            If t.Y < -MaxSize Then
                t.Y = Host.Height + MaxSize
            End If
            ' Check if the particle has gone outside
            If t.X < -MaxSize OrElse t.X > Host.Width + MaxSize Then
                t.X = random.NextDouble() * Host.Width
                t.Y = Host.Height + MaxSize
            End If
            ' Brush & Effect
            ellipse.Fill = p.Brush
            ' Comment this line to deactivate the Blur Effect
            If Blurry Then
                ellipse.Effect = p.Blur
            End If
        Next
    End Sub

    Private Sub CreateParticle()
        ' Brush (White)
        Dim brush = Color
        ' Opacity (0.2 <= 1)
        brush.Opacity = 0.2 + random.NextDouble() * 0.8
        ' Blur effect
        Dim blur = New BlurEffect()
        blur.RenderingBias = RenderingBias.Performance
        ' Radius (1 <= 40)
        blur.Radius = 1 + random.NextDouble() * 39
        ' Ellipse
        Dim ellipse = New Ellipse()
        ellipse.Width = MaxSize * 0.15 + random.NextDouble() * MaxSize * 0.8
        ' Size (from 15% to 95% of MaxSize)
        ellipse.Height = ellipse.Width
        ' Starting location of the ellipse (anywhere in the screen)
        ellipse.RenderTransform = New TranslateTransform(random.NextDouble() * Host.Width, random.NextDouble() * Host.Height)
        ellipse.Tag = New Particle With {
            .Blur = blur,
            .Brush = brush,
            .Velocity = New Point With {
                .X = HorizontalVelocity + random.NextDouble() * 4,
                .Y = VerticalVelocity + random.NextDouble() * 2
            }
        }
        ' Add the ellipse to the Canvas
        ParticleHost.Children.Add(ellipse)
    End Sub

    Public Class Particle
        Public Property Velocity As Point ' Speed Of move
        Public Property Blur As BlurEffect ' Blur effect
        Public Property Brush As Brush ' Brush (opacity)
    End Class
End Class
