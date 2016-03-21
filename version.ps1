if ($args.Length -eq 0)
{
	exit 0
}

$Version = $args[0]

if ($args.Length -ge 2)
{
	$Path = $args[1].Trim("""") + '\version.xml'
}
else
{
	exit 0;
}

$Parts = $Version.Split(".")
 # I keep getting "Access to the path ...\version.xml' is denied." error.. remove readonly
 sp $Path IsReadOnly $false
# get an XMLTextWriter to create the XML
$XmlWriter = New-Object System.XMl.XmlTextWriter($Path,$Null)
 
# choose a pretty formatting:
$xmlWriter.Formatting = "Indented"
$xmlWriter.Indentation = 1
$XmlWriter.IndentChar = " "
 
# write the header
$xmlWriter.WriteStartDocument()
  
# create root element "machines" and add some attributes to it
$XmlWriter.WriteComment('Portal version info')
$xmlWriter.WriteStartElement('vesrion')

if ($Parts.Length -ge 1)
{
	$XmlWriter.WriteStartElement('major')
	$XmlWriter.WriteString($Parts[0])
	$XmlWriter.WriteEndElement();
}

if ($Parts.Length -ge 2)
{
	$XmlWriter.WriteStartElement('minor')
	$XmlWriter.WriteString($Parts[1])
	$XmlWriter.WriteEndElement();
}

if ($Parts.Length -ge 3)
{
	$XmlWriter.WriteStartElement('build')
	$XmlWriter.WriteString($Parts[2])
	$XmlWriter.WriteEndElement();
}

if ($Parts.Length -ge 4)
{
	$XmlWriter.WriteStartElement('revision')
	$XmlWriter.WriteString($Parts[3])
	$XmlWriter.WriteEndElement();
}

$XmlWriter.WriteEndElement();
 
# finalize the document:
$xmlWriter.WriteEndDocument()
$xmlWriter.Flush()
$xmlWriter.Close()

[Console]::WriteLine("version.xml has been created in {0}", $Path);

exit 0
