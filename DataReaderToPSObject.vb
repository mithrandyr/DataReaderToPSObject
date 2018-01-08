Public Class DataReaderToPSObject
    Private Class Map
        Public Ordinal As Integer
        Public Name As String
        'Public AllowNull As Boolean
        Public DataType As String

        Public Sub New(ordinal As Integer, datatype As String, Optional name As String = Nothing) 'allownull As Boolean, 
            If String.IsNullOrWhiteSpace(name) Then name = String.Format("Column{0}", ordinal + 1)

            Me.Ordinal = ordinal
            Me.Name = name
            'Me.AllowNull = allownull
            Me.DataType = datatype
        End Sub

    End Class

    Public Shared Iterator Function Translate(dataReader As IDataReader) As IEnumerable(Of PSObject)
        Dim MapList As New List(Of Map), Ord As Integer = 0
        For Each x In dataReader.GetSchemaTable.AsEnumerable.OrderBy(Function(r) r("ColumnOrdinal")).ToList
            MapList.Add(New Map(Ord, x("DataType").Name, x("ColumnName"))) 'x("AllowDBNull"),
            Ord += 1
        Next


        While dataReader.Read
            Dim psObj As New PSObject
            For Each m As Map In MapList
                With psObj.Members
                    If dataReader.IsDBNull(m.Ordinal) Then 'm.AllowNull AndAlso dataReader.IsDBNull(m.Ordinal) Then 
                        .Add(New PSNoteProperty(m.Name, Nothing), True)
                    Else
                        Try
                            Select Case m.DataType
                                Case "Boolean"
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetBoolean(m.Ordinal)), True)
                                Case "Byte"
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetByte(m.Ordinal)), True)
                                Case "Char"
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetChar(m.Ordinal)), True)
                                Case "DateTime"
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetDateTime(m.Ordinal)), True)
                                Case "Decimal"
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetDecimal(m.Ordinal)), True)
                                Case "Double"
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetDouble(m.Ordinal)), True)
                                Case "Single"
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetFloat(m.Ordinal)), True)
                                Case "Guid"
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetGuid(m.Ordinal)), True)
                                Case "Int16"
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetInt16(m.Ordinal)), True)
                                Case "Int32"
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetInt32(m.Ordinal)), True)
                                Case "Int64"
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetInt64(m.Ordinal)), True)
                                Case "String"
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetString(m.Ordinal)), True)
                                Case Else
                                    .Add(New PSNoteProperty(m.Name, dataReader.GetValue(m.Ordinal)), True)
                            End Select
                        Catch ex As Exception
                            Dim msg As String = String.Format("Failed to translate, ColumnName = {0} | ColumnOrdinal = {1} | ColumnType = {2} | ToStringValue = '{3}' | See InnerException for details", m.Name, m.Ordinal, m.DataType, dataReader.GetValue(m.Ordinal).ToString)
                            Throw New Exception(msg, ex)
                        End Try
                    End If
                End With
            Next
            Yield psObj
        End While

    End Function
End Class
