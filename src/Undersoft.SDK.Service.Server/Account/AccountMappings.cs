using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Undersoft.SDK.Service.Server.Accounts
{
    using Undersoft.SDK.Service.Data.Store;
    using Undersoft.SDK.Service.Infrastructure.Database;

    public class AccountMappings : EntityTypeMapping<Account>
    {
        const string TABLE_NAME = "Accounts";

        public override void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable(TABLE_NAME, DataStoreSchema.DomainSchema);

            builder
                .HasMany<Role>(r => r.Roles)
                .WithMany(a => a.Accounts)
                .UsingEntity<AccountRole>(
                    j => j.HasOne(a => a.Role).WithMany().HasForeignKey(r => r.AccountRoleId),
                    j => j.HasOne(a => a.Account).WithMany().HasForeignKey(r => r.AccountId),
                    j =>
                    {
                        j.HasKey(k => new { k.AccountId, k.AccountRoleId });
                    }
                );

            builder.HasMany(c => c.Claims).WithOne(c => c.Account).HasForeignKey(c => c.AccountId);

            builder.HasMany(c => c.Tokens).WithOne(c => c.Account).HasForeignKey(c => c.AccountId);

            builder.HasOne(c => c.User).WithOne().HasForeignKey<Account>(u => u.UserId);
        }
    }

    public class RolemMappings : EntityTypeMapping<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasMany(c => c.Claims).WithOne(c => c.Role).HasForeignKey(c => c.AccountRoleId);
        }
    }

    public class AccountClaimMappings : EntityTypeMapping<AccountClaim>
    {
        public override void Configure(EntityTypeBuilder<AccountClaim> builder)
        {
            builder.HasOne(c => c.Account).WithMany(c => c.Claims).HasForeignKey(c => c.AccountId);
        }
    }

    public class AccountTokenMappings : EntityTypeMapping<AccountToken>
    {
        public override void Configure(EntityTypeBuilder<AccountToken> builder)
        {
            builder.HasOne(c => c.Account).WithMany(c => c.Tokens).HasForeignKey(c => c.AccountId);
        }
    }
}
