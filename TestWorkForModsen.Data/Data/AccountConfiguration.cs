using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Data.Data
{
    public class AccountConfiguration : EntityConfiguration<Account>
    {
        public override void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<Account>(a => a.UserId);
        }
    }
}
