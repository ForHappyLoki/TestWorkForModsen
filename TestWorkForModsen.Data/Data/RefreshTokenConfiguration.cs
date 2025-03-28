using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Data.Data
{
    public class RefreshTokenConfiguration : EntityConfiguration<RefreshToken>
    {
        public override void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasOne(rt => rt.Account)
                .WithMany(a => a.RefreshTokens)
                .HasForeignKey(rt => rt.AccountId);
        }
    }
}
