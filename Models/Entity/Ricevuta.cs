using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Scadenzario.Models.Entities;

public class Ricevuta
{
    public int Id { get; set; }
    public int IdScadenza { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string Path {get;set;}
    public byte[] FileContent { get; set; }
    public string Beneficiario { get; set; }

    public virtual Scadenza Scadenza { get; set; }
}