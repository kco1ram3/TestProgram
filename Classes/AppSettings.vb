Public Class AppSettings
    Public Shared Function UseSessionForStorageData() As Boolean
        Return ConfigurationManager.AppSettings.Get("UseSessionForStorageData")
    End Function
End Class
