using Modules.MyQCard.Repository;
using Modules.QCard.Repository;
using QCard.ManagementSystem.Domain.Modules.Core.Repository;
using QCard.ManagementSystem.Domain.Modules.Monitoring.Repository;
using QCard.ManagementSystem.Domain.Modules.QCard.Repository;
using System.Data;

namespace QCard.ManagementSystem.Domain
{
    public partial interface IDbHelper : IDisposable
    {
        // Connection and Transaction Object  
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }

        // Model Repositories 
        IDataSourceRepository DataSourceRepository { get; }
        ILogActivityRepository LogActivityRepository { get; }
        IFunctionRepository FunctionRepository { get; }
        IUserRepository UserRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        IRoleRepository RoleRepository { get; }
        IMenuGroupRepository MenuGroupRepository { get; }
        ISettingRepository SettingRepository { get; }
        IMenuRepository MenuRepository { get; }
        IAuthenticationLogRepository AuthenticationLogRepository { get; }
        IConfigRepository ConfigRepository { get; }
        IRolePermissionRepository RolePermissionRepository { get; }
        IFileRepository FileRepository { get; }
        IControlBatchProcessRepository ControlBatchProcessRepository { get; }
        IMyQCardRepository MyQCardRepository { get; }
        IMyMachinesRepository MyMachinesRepository { get; }
        IProgramRepository ProgramRepository { get; }
        ISerialNoRepository SerialNoRepository { get; }
        IQCardListRepository QCardListRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        IModelRepository ModelRepository { get; }
        IPartNoRepository PartNoRepository { get; }
        IClaimHistoryRepository ClaimHistoryRepository { get; }
        IClaimProcessRepository ClaimProcessRepository { get; }
        IDashboardRepository DashboardRepository { get; }
        ITaskApprovalRepository TaskApprovalRepository { get; }
    }
    public class DbHelper : DbContext, IDbHelper
    {
        public DbHelper(bool UseTransaction = false) : base(UseTransaction)
        {
        }

        public IDbConnection Connection
        {
            get
            {
                return base._connection;
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                return base._transaction;
            }
        }

        private IDataSourceRepository _DataSourceRepository;
        private ILogActivityRepository _LogActivityRepository;
        private IMessageRepository _MessageRepository;
        private IUserRepository _UserRepository;
        private IUserRoleRepository _UserRoleRepository;
        private IRoleRepository _RoleRepository;
        private IMenuGroupRepository _MenuGroupRepository;
        private ISettingRepository _SettingRepository;
        private IMenuRepository _MenuRepository;
        private IFunctionRepository _FunctionRepository;
        private IAuthenticationLogRepository _AuthenticationLogRepository;
        private IConfigRepository _ConfigRepository;
        private IRolePermissionRepository _RolePermissionRepository;
        private IFileRepository _FileRepository;
        private IControlBatchProcessRepository _ControlBatchProcessRepository;
        private IMyQCardRepository _MyQCardRepository;
        private IQCardListRepository _QCardListRepository;
        private ICustomerRepository _CustomerRepository;
        private IMyMachinesRepository _MyMachinesRepository;
        private IProgramRepository _ProgramRepository;
        private IModelRepository _ModelRepository;
        private IPartNoRepository _PartNoRepository;
        private ISerialNoRepository _SerialNoRepository;
        private IClaimHistoryRepository _ClaimHistoryRepository;
        private IDashboardRepository _DashboardRepository;
        private ITaskApprovalRepository _TaskApprovalRepository;
        private IClaimProcessRepository _ClaimProcessRepository;

        public IDataSourceRepository DataSourceRepository
        {
            get { return _DataSourceRepository ?? (_DataSourceRepository = new DataSourceRepository(_connection, _transaction)); }
        }
        public ILogActivityRepository LogActivityRepository
        {
            get { return _LogActivityRepository ?? (_LogActivityRepository = new LogActivityRepository(_connection, _transaction)); }
        }
        public IFunctionRepository FunctionRepository
        {
            get { return _FunctionRepository ?? (_FunctionRepository = new FunctionRepository(_connection, _transaction)); }
        }
        public IMessageRepository MessageRepository
        {
            get { return _MessageRepository ?? (_MessageRepository = new MessageRepository(_connection, _transaction)); }
        }
        public IUserRepository UserRepository
        {
            get { return _UserRepository ?? (_UserRepository = new UserRepository(_connection, _transaction)); }
        }
        public IUserRoleRepository UserRoleRepository
        {
            get { return _UserRoleRepository ?? (_UserRoleRepository = new UserRoleRepository(_connection, _transaction)); }
        }
        public IRoleRepository RoleRepository
        {
            get { return _RoleRepository ?? (_RoleRepository = new RoleRepository(_connection, _transaction)); }
        }
        public IMenuGroupRepository MenuGroupRepository
        {
            get { return _MenuGroupRepository ?? (_MenuGroupRepository = new MenuGroupRepository(_connection, _transaction)); }
        }
        public ISettingRepository SettingRepository
        {
            get { return _SettingRepository ?? (_SettingRepository = new SettingRepository(_connection, _transaction)); }
        }
        public IMenuRepository MenuRepository
        {
            get { return _MenuRepository ?? (_MenuRepository = new MenuRepository(_connection, _transaction)); }
        }
        public IConfigRepository ConfigRepository
        {
            get { return _ConfigRepository ?? (_ConfigRepository = new ConfigRepository(_connection, _transaction)); }
        }
        public IAuthenticationLogRepository AuthenticationLogRepository
        {
            get { return _AuthenticationLogRepository ?? (_AuthenticationLogRepository = new AuthenticationLogRepository(_connection, _transaction)); }
        }
        public IRolePermissionRepository RolePermissionRepository
        {
            get { return _RolePermissionRepository ?? (_RolePermissionRepository = new RolePermissionRepository(_connection, _transaction)); }
        }
        public IFileRepository FileRepository
        {
            get { return _FileRepository ?? (_FileRepository = new FileRepository(_connection, _transaction)); }
        }
        public IControlBatchProcessRepository ControlBatchProcessRepository
        {
            get { return _ControlBatchProcessRepository ?? (_ControlBatchProcessRepository = new ControlBatchProcessRepository(_connection, _transaction)); }
        }
        public IMyQCardRepository MyQCardRepository
        {
            get { return _MyQCardRepository ?? (_MyQCardRepository = new MyQCardRepository(_connection, _transaction)); }
        }
        public IQCardListRepository QCardListRepository
        {
            get { return _QCardListRepository ?? (_QCardListRepository = new QCardListRepository(_connection, _transaction)); }
        }
        public ICustomerRepository CustomerRepository
        {
            get { return _CustomerRepository ?? (_CustomerRepository = new CustomerRepository(_connection, _transaction)); }
        }
        public IMyMachinesRepository MyMachinesRepository
        {
            get { return _MyMachinesRepository ?? (_MyMachinesRepository = new MyMachinesRepository(_connection, _transaction)); }
        }
        public IProgramRepository ProgramRepository
        {
            get { return _ProgramRepository ?? (_ProgramRepository = new ProgramRepository(_connection, _transaction)); }
        }
        public IModelRepository ModelRepository
        {
            get { return _ModelRepository ?? (_ModelRepository = new ModelRepository(_connection, _transaction)); }
        }
        public IPartNoRepository PartNoRepository
        {
            get { return _PartNoRepository ?? (_PartNoRepository = new PartNoRepository(_connection, _transaction)); }
        }
        public ISerialNoRepository SerialNoRepository
        {
            get { return _SerialNoRepository ?? (_SerialNoRepository = new SerialNoRepository(_connection, _transaction)); }
        }
        public IClaimHistoryRepository ClaimHistoryRepository
        {
            get { return _ClaimHistoryRepository ?? (_ClaimHistoryRepository = new ClaimHistoryRepository(_connection, _transaction)); }
        }
        public IClaimProcessRepository ClaimProcessRepository
        {
            get { return _ClaimProcessRepository ?? (_ClaimProcessRepository = new ClaimProcessRepository(_connection, _transaction)); }
        }
        public IDashboardRepository DashboardRepository
        {
            get { return _DashboardRepository ?? (_DashboardRepository = new DashboardRepository(_connection, _transaction)); }
        }
        public ITaskApprovalRepository TaskApprovalRepository
        {
            get { return _TaskApprovalRepository ?? (_TaskApprovalRepository = new TaskApprovalRepository(_connection, _transaction)); }
        }

        

        public override void resetRepositories()
        {
            _DataSourceRepository = null;
            _LogActivityRepository = null;
            _MessageRepository = null;
            _UserRepository = null;
            _UserRoleRepository = null;
            _RoleRepository = null;
            _FunctionRepository = null;
            _MenuGroupRepository = null;
            _MenuRepository = null;
            _AuthenticationLogRepository = null;
            _ConfigRepository = null;
            _RolePermissionRepository = null;
            _FileRepository = null;
            _ControlBatchProcessRepository = null;
            _MyQCardRepository = null;
            _QCardListRepository = null;
            _CustomerRepository = null;
            _MyMachinesRepository = null;
            _ProgramRepository = null;
            _ModelRepository = null;
            _PartNoRepository = null;
            _SerialNoRepository = null;
            _ClaimHistoryRepository = null;
            _ClaimProcessRepository = null;
            _DashboardRepository = null;
            _TaskApprovalRepository = null;
        }
    }
}