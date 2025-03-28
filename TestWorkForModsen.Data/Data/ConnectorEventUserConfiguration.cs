using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Data.Data
{
    public class ConnectorEventUserConfiguration : EntityConfiguration<ConnectorEventUser>
    {
        public override void Configure(EntityTypeBuilder<ConnectorEventUser> builder)
        {
            builder.HasKey(ceu => new { ceu.EventId, ceu.UserId });

            builder.HasOne(ceu => ceu.Event)
                .WithMany(e => e.ConnectorEventUser)
                .HasForeignKey(ceu => ceu.EventId);

            builder.HasOne(ceu => ceu.User)
                .WithMany(u => u.ConnectorEventUser)
                .HasForeignKey(ceu => ceu.UserId);
        }
    }
}
