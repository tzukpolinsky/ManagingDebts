using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ManagingDebts
{
    public partial class ManagingDebtsContext : DbContext
    {
        private readonly string connectionString;
        public ManagingDebtsContext()
        {
        }

        public ManagingDebtsContext(string connectionString)
            : base()
        {
            this.connectionString = connectionString;
        }

        public virtual DbSet<BankAccounts> BankAccounts { get; set; }
        public virtual DbSet<BezekFileInfo> BezekFileInfo { get; set; }
        public virtual DbSet<Budgets> Budgets { get; set; }
        public virtual DbSet<BudgetsContracts> BudgetsContracts { get; set; }
        public virtual DbSet<Contracts> Contracts { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<ElectricityFileInfo> ElectricityFileInfo { get; set; }
        public virtual DbSet<GeneralBillingSummary> GeneralBillingSummary { get; set; }
        public virtual DbSet<PrivateSupplierFileInfo> PrivateSupplierFileInfo { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankAccounts>(entity =>
            {
                entity.HasKey(e => new { e.BankAccountInFinance, e.CustomerId, e.SupplierId })
                    .HasName("PK_bank_accounts_1");

                entity.ToTable("bank_accounts");

                entity.Property(e => e.BankAccountInFinance).HasColumnName("bank_account_in_finance");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.SupplierId)
                    .HasColumnName("supplier_id")
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.HasOne(d => d.Suppliers)
                    .WithMany(p => p.BankAccounts)
                    .HasForeignKey(d => new { d.SupplierId, d.CustomerId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bank_accounts_suppliers");
            });

            modelBuilder.Entity<BezekFileInfo>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.GeneralRowId, e.RowId });

                entity.ToTable("bezek_file_info");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.GeneralRowId).HasColumnName("general_row_id");

                entity.Property(e => e.RowId).HasColumnName("row_id");

                entity.Property(e => e.BillingAmount)
                    .HasColumnName("billing_amount")
                    .HasColumnType("money");

                entity.Property(e => e.BillingAmountAfterTax)
                    .HasColumnName("billing_amount_after_tax")
                    .HasColumnType("money");

                entity.Property(e => e.BillingDescription)
                    .IsRequired()
                    .HasColumnName("billing_description")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BillingType)
                    .IsRequired()
                    .HasColumnName("billing_type")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CallRate)
                    .HasColumnName("call_rate")
                    .HasColumnType("money");

                entity.Property(e => e.CallTime)
                    .HasColumnName("call_time")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CallsAmount).HasColumnName("calls_amount");

                entity.Property(e => e.ClientNumber)
                    .IsRequired()
                    .HasColumnName("client_number")
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.ConsumptionAmount).HasColumnName("consumption_amount");

                entity.Property(e => e.DepartmentNumber)
                    .IsRequired()
                    .HasColumnName("department_number")
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.DiscountPrecent).HasColumnName("discount_precent");

                entity.Property(e => e.EndDateBilling)
                    .HasColumnName("end_date_billing")
                    .HasColumnType("datetime");

                entity.Property(e => e.FreeTimeUsage)
                    .HasColumnName("free_time_usage")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FreeTimeUsageSupplier)
                    .HasColumnName("free_time_usage_supplier")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.HebServiceType)
                    .HasColumnName("heb_service_type")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.InvoiceNumber)
                    .IsRequired()
                    .HasColumnName("invoice_number")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.IsMatched).HasColumnName("is_matched");

                entity.Property(e => e.JournalEntryNumber).HasColumnName("journal_entry_number");

                entity.Property(e => e.MonthlyRate)
                    .HasColumnName("monthly_rate")
                    .HasColumnType("money");

                entity.Property(e => e.OriginalClient)
                    .IsRequired()
                    .HasColumnName("original_client")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.OriginalPayer).HasColumnName("original_payer");

                entity.Property(e => e.PayerNumberBezek).HasColumnName("payer_number_bezek");

                entity.Property(e => e.PriceBeforeDiscount)
                    .HasColumnName("price_before_discount")
                    .HasColumnType("money");

                entity.Property(e => e.SecondaryServiceType)
                    .HasColumnName("secondary_service_type")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceType)
                    .HasColumnName("service_type")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.StartDateBilling)
                    .HasColumnName("start_date_billing")
                    .HasColumnType("datetime");

                entity.Property(e => e.SubscriptionNumber).HasColumnName("subscription_number");

                entity.Property(e => e.TaxRate).HasColumnName("tax_rate");

                entity.Property(e => e.TimePeriodText)
                    .HasColumnName("time_period_text")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.GeneralRow)
                    .WithMany(p => p.BezekFileInfo)
                    .HasForeignKey(d => d.GeneralRowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bezek_file_info_general_billing_summary");
            });

            modelBuilder.Entity<Budgets>(entity =>
            {
                entity.HasKey(e => new { e.BudgetId, e.CustomerId, e.SupplierId });

                entity.ToTable("budgets");

                entity.Property(e => e.BudgetId).HasColumnName("budget_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.SupplierId)
                    .HasColumnName("supplier_id")
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.BudgetName)
                    .IsRequired()
                    .HasColumnName("budget_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Suppliers)
                    .WithMany(p => p.Budgets)
                    .HasForeignKey(d => new { d.SupplierId, d.CustomerId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_budgets_suppliers");
            });

            modelBuilder.Entity<BudgetsContracts>(entity =>
            {
                entity.HasKey(e => new { e.BudgetId, e.ContractId, e.CustomerId, e.SupplierId });

                entity.ToTable("budgets_contracts");

                entity.Property(e => e.BudgetId).HasColumnName("budget_id");

                entity.Property(e => e.ContractId).HasColumnName("contract_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.SupplierId)
                    .HasColumnName("supplier_id")
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.BudgetPrecent).HasColumnName("budget_precent");

                entity.HasOne(d => d.Budgets)
                    .WithMany(p => p.BudgetsContracts)
                    .HasForeignKey(d => new { d.BudgetId, d.CustomerId, d.SupplierId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_budgets_contracts_budgets");

                entity.HasOne(d => d.Contracts)
                    .WithMany(p => p.BudgetsContracts)
                    .HasForeignKey(d => new { d.ContractId, d.CustomerId, d.SupplierId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_budgets_contracts_contracts");
            });

            modelBuilder.Entity<Contracts>(entity =>
            {
                entity.HasKey(e => new { e.ContractId, e.CustomerId, e.SupplierId });

                entity.ToTable("contracts");

                entity.Property(e => e.ContractId).HasColumnName("contract_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.SupplierId)
                    .HasColumnName("supplier_id")
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.BankAccountInFinance).HasColumnName("bank_account_in_finance");

                entity.Property(e => e.ContractAddress)
                    .HasColumnName("contract_address")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ContractDescription)
                    .IsRequired()
                    .HasColumnName("contract_description")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_contracts_customers");

                entity.HasOne(d => d.Suppliers)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => new { d.SupplierId, d.CustomerId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_contracts_suppliers");
            });

            modelBuilder.Entity<Customers>(entity =>
            {
                entity.HasKey(e => e.FinanceId);

                entity.ToTable("customers");

                entity.Property(e => e.FinanceId)
                    .HasColumnName("finance_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsBusiness).HasColumnName("is_business");

                entity.Property(e => e.MgaName)
                    .IsRequired()
                    .HasColumnName("mga_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ElectricityFileInfo>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.GeneralRowId, e.RowId });

                entity.ToTable("electricity_file_info");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.GeneralRowId).HasColumnName("general_row_id");

                entity.Property(e => e.RowId).HasColumnName("row_id");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("money");

                entity.Property(e => e.BankAccount).HasColumnName("bank_account");

                entity.Property(e => e.BankAccountType).HasColumnName("bank_account_type");

                entity.Property(e => e.BankBranch).HasColumnName("bank_branch");

                entity.Property(e => e.BankCode).HasColumnName("bank_code");

                entity.Property(e => e.BillCreatingDate)
                    .HasColumnName("bill_creating_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ConsumerAddress)
                    .IsRequired()
                    .HasColumnName("consumer_address")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ConsumerName)
                    .IsRequired()
                    .HasColumnName("consumer_name")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ConsumerNumber).HasColumnName("consumer_number");

                entity.Property(e => e.Contract).HasColumnName("contract");

                entity.Property(e => e.Invoice).HasColumnName("invoice");

                entity.Property(e => e.IsMatched).HasColumnName("is_matched");

                entity.Property(e => e.JournalEntryNumber).HasColumnName("journal_entry_number");

                entity.Property(e => e.MonthOfLastInvoice).HasColumnName("month_of_last_invoice");

                entity.Property(e => e.NumberOfCreditDays).HasColumnName("number_of_credit_days");

                entity.Property(e => e.PaymentDate)
                    .HasColumnName("payment_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.YearOfLastInvoice).HasColumnName("year_of_last_invoice");

                entity.HasOne(d => d.GeneralRow)
                    .WithMany(p => p.ElectricityFileInfo)
                    .HasForeignKey(d => d.GeneralRowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_electricity_file_info_general_billing_summary");
            });

            modelBuilder.Entity<GeneralBillingSummary>(entity =>
            {
                entity.HasKey(e => e.RowId);

                entity.ToTable("general_billing_summary");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("IX_general_billing_summary");

                entity.Property(e => e.RowId)
                    .HasColumnName("row_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.BillFromDate)
                    .HasColumnName("bill_from_date")
                    .HasColumnType("date");

                entity.Property(e => e.BillToDate)
                    .HasColumnName("bill_to_date")
                    .HasColumnType("date");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.DateOfValue)
                    .HasColumnName("date_of_value")
                    .HasColumnType("date");

                entity.Property(e => e.Sent).HasColumnName("sent");

                entity.Property(e => e.SupplierClientNumber).HasColumnName("supplier_client_number");

                entity.Property(e => e.SupplierId)
                    .IsRequired()
                    .HasColumnName("supplier_id")
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierPayerId).HasColumnName("supplier_payer_id");

                entity.Property(e => e.TotalChangableBilling)
                    .HasColumnName("total_changable_billing")
                    .HasColumnType("money");

                entity.Property(e => e.TotalCredit)
                    .HasColumnName("total_credit")
                    .HasColumnType("money");

                entity.Property(e => e.TotalDebit)
                    .HasColumnName("total_debit")
                    .HasColumnType("money");

                entity.Property(e => e.TotalExtraPayments)
                    .HasColumnName("total_extra_payments")
                    .HasColumnType("money");

                entity.Property(e => e.TotalFixedBilling)
                    .HasColumnName("total_fixed_billing")
                    .HasColumnType("money");

                entity.Property(e => e.TotalInvoice)
                    .HasColumnName("total_invoice")
                    .HasColumnType("money");

                entity.Property(e => e.TotalInvoiceBeforeTax)
                    .HasColumnName("total_invoice_before_tax")
                    .HasColumnType("money");

                entity.Property(e => e.TotalOneTimeBilling)
                    .HasColumnName("total_one_time_billing")
                    .HasColumnType("money");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.GeneralBillingSummary)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_general_billing_summary_customers");

                entity.HasOne(d => d.Suppliers)
                    .WithMany(p => p.GeneralBillingSummary)
                    .HasForeignKey(d => new { d.SupplierId, d.CustomerId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_general_billing_summary_suppliers");
            });

            modelBuilder.Entity<PrivateSupplierFileInfo>(entity =>
            {
                entity.HasKey(e => new { e.SupplierId, e.CustomerId, e.RowId, e.GeneralRowId });

                entity.ToTable("private_supplier_file_info");

                entity.Property(e => e.SupplierId)
                    .HasColumnName("supplier_id")
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.RowId).HasColumnName("row_id");

                entity.Property(e => e.GeneralRowId).HasColumnName("general_row_id");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("money");

                entity.Property(e => e.AmountAfterTax)
                    .HasColumnName("amount_after_tax")
                    .HasColumnType("money");

                entity.Property(e => e.Contract).HasColumnName("contract");

                entity.Property(e => e.DateOfValue)
                    .HasColumnName("date_of_value")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Invoice).HasColumnName("invoice");

                entity.Property(e => e.IsMatched).HasColumnName("is_matched");

                entity.Property(e => e.JournalEntryNumber).HasColumnName("journal_entry_number");

                entity.Property(e => e.TaxRate).HasColumnName("tax_rate");

                entity.HasOne(d => d.GeneralRow)
                    .WithMany(p => p.PrivateSupplierFileInfo)
                    .HasForeignKey(d => d.GeneralRowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_private_supplier_file_info_general_billing_summary");
            });

            modelBuilder.Entity<Suppliers>(entity =>
            {
                entity.HasKey(e => new { e.SupplierId, e.SupplierCustomerId });

                entity.ToTable("suppliers");

                entity.Property(e => e.SupplierId)
                    .HasColumnName("supplier_id")
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierCustomerId).HasColumnName("supplier_customer_id");

                entity.Property(e => e.SupplierEnabled).HasColumnName("supplier_enabled");

                entity.Property(e => e.SupplierName)
                    .IsRequired()
                    .HasColumnName("supplier_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierNumberInFinance).HasColumnName("supplier_number_in_finance");

                entity.Property(e => e.SupplierPkudatYomanNumber).HasColumnName("supplier_pkudat_yoman_number");

                entity.Property(e => e.SupplierWithBanks).HasColumnName("supplier_with_banks");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_users_1");

                entity.ToTable("users");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.UserEmail)
                    .HasColumnName("user_email")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserFirstName)
                    .IsRequired()
                    .HasColumnName("user_first_name")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.UserIsActive).HasColumnName("user_is_active");

                entity.Property(e => e.UserIsSuperAdmin).HasColumnName("user_is_super_admin");

                entity.Property(e => e.UserLastName)
                    .IsRequired()
                    .HasColumnName("user_last_name")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasColumnName("user_password")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_users_customers");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
