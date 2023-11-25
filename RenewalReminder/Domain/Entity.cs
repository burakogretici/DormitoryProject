﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenewalReminder.Domain
{
	public class Entity
	{
        [Key]
        public int Id { get; set; }

        public bool Deleted { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }

        public Entity()
        {
            Deleted = false;
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Id == ((Entity)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}

