# InfraBase Microservice - Domain, Application & Infrastructure Layers

## Overview
This document describes the implementation of the InfraBase microservice following Domain-Driven Design (DDD) principles, implementing the "Add Request" and "PC Admin Approval/Rejection" user stories.

## Architecture

### Domain Layer (`PartnersHub.InfraBase.Domain`)

#### Common Base Classes
- **Entity**: Base class for all entities with identity
- **AggregateRoot**: Base class for aggregate roots with domain events
- **ValueObject**: Base class for value objects
- **DomainEvent**: Base class for domain events
- **Result<T>**: Result pattern for error handling

#### Enums
- **RequestStatus**: Draft, PendingPcAdminApproval, PendingPcInfrabaseApproval, AcceptedByPcAdmin, RejectedByPcAdmin, Approved, Rejected
- **TenderingStage**: PreTender, Tendered, Award, Execution, Delivered
- **FundingModel**: FullySelfFunded, PublicPrivatePartnership, FullyGovernmentFunded, JointVenture
- **AmountType**: CAPEX, OPEX

#### Value Objects
- **ProjectName**: Max 500 characters, required, unique
- **ProjectDescription**: Max 3000 characters, optional
- **ItemName**: Max 500 characters, required, unique per request
- **Quantity**: Max 5 digits, positive decimal, no zero
- **UnitPrice**: Max 10 digits, positive decimal, no zero
- **YearlyAmount**: Max 11 digits, positive decimal, no zero
- **RejectionReason**: Max 3000 characters, required for rejections

#### Aggregates

**InfrabaseRequest** (Aggregate Root)
- Properties:
  - RequestCode (auto-generated on submit)
  - ProjectName, ProjectDescription
  - SectorId, SubSectorId, AssetTypeId (lookups)
  - TenderingStage, FundingModel
  - Status, TotalRequestAmount
  - CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
  - SubmittedBy, SubmittedAt
  - RejectionReason, RejectedBy, RejectedAt
  - ApprovedBy, ApprovedAt
  - **History** (Collection of RequestHistory)

- Methods:
  - `Create()`: Create new draft request (tracks history)
  - `AddItem()`: Add item to request (validates uniqueness)
  - `RemoveItem()`: Remove item from request
  - `UpdateItem()`: Update item quantity or price
  - `SaveAsDraft()`: Save without validation (tracks history)
  - `Submit()`: Submit for approval (generates code, validates items, tracks history)
  - `AcceptByPcAdmin()`: Accept request (PC Admin) (tracks history)
  - `RejectByPcAdmin()`: Reject request with reason (PC Admin) (tracks history)
  - `UpdateBasicInformation()`: Update request details (draft only) (tracks history with field changes)

**RequestItem** (Entity)
- Properties:
  - ItemCode (auto-generated)
  - ItemName, UomId
  - Quantity, UnitPrice, TotalAmount
  - FinancialDistributions collection

- Methods:
  - `Create()`: Create new item
  - `UpdateQuantity()`: Update quantity (recalculates total)
  - `UpdateUnitPrice()`: Update price (recalculates total)
  - `AddFinancialDistribution()`: Add CAPEX/OPEX distribution
  - `ValidateFinancialDistributions()`: Ensure distributions match total

**FinancialDistribution** (Entity)
- Properties:
  - AmountType (CAPEX/OPEX)
  - Year (1-99)
  - Amount

**RequestHistory** (Entity) ? NEW
- Properties:
  - RequestId
  - Status (at time of action)
  - Action (Created, Updated, Submitted, Accepted, Rejected, etc.)
  - Comments
  - PerformedBy
  - PerformedAt
  - FieldsChanged (comma-separated field names)
  - OldValues (comma-separated old values)
  - NewValues (comma-separated new values)

- Tracks:
  - Request creation
  - Draft saves
  - Submissions
  - Basic information updates (with field-level changes)
  - PC admin acceptances
  - PC admin rejections

#### Domain Events
- **RequestCreatedEvent**: Raised when request is created
- **RequestSavedAsDraftEvent**: Raised when saved as draft
- **RequestSubmittedEvent**: Raised when submitted
- **RequestItemAddedEvent**: Raised when item is added
- **RequestAcceptedByPcAdminEvent**: Raised when PC admin accepts
- **RequestRejectedByPcAdminEvent**: Raised when PC admin rejects

### Application Layer (`PartnersHub.InfraBase.Application`)

#### Interfaces

**IUnitOfWork**
- `SaveChangesAsync()`: Commit changes to database

**IInfrabaseRequestRepository**
- `GetByIdAsync()`: Get request by ID (includes history)
- `GetByIdWithItemsAsync()`: Get request with items, distributions, and history
- `GetAllAsync()`: Get all requests (includes history)
- `GetByStatusAsync()`: Get requests by status (includes history)
- `GetByCreatedByAsync()`: Get requests by user (includes history)
- `IsProjectNameUniqueAsync()`: Check project name uniqueness
- `GetNextRequestCodeAsync()`: Generate next request code
- `GetNextItemCodeAsync()`: Generate next item code
- `AddAsync()`, `Update()`, `Delete()`: CRUD operations

**ILookupRepository**
- `SectorExistsAsync()`: Validate sector
- `SubSectorExistsAsync()`: Validate sub-sector with sector
- `AssetTypeExistsAsync()`: Validate asset type
- `UomExistsAsync()`: Validate unit of measurement

#### Commands & Handlers

1. **CreateRequestCommand**: Create new draft request
   - Validates lookups exist
   - Validates project name uniqueness
   - Creates request in draft status
   - Adds creation history entry

2. **AddRequestItemCommand**: Add item to request
   - Validates UOM exists
   - Generates item code
   - Validates item name uniqueness within request

3. **SaveRequestAsDraftCommand**: Save request as draft
   - Updates timestamp
   - Adds history entry
   - Raises domain event

4. **SubmitRequestCommand**: Submit request for approval
   - Validates items exist
   - Validates financial distributions
   - Generates request code
   - Sets status based on user role
   - Adds submission history entry

5. **AddFinancialDistributionCommand**: Add CAPEX/OPEX distribution to item
   - Validates year range
   - Validates amount

6. **AcceptRequestByPcAdminCommand**: Accept request (PC Admin only)
   - Validates status is PendingPcAdminApproval
   - Changes status to AcceptedByPcAdmin
   - Records approval timestamp and user
   - Adds acceptance history entry

7. **RejectRequestByPcAdminCommand**: Reject request (PC Admin only)
   - Validates status is PendingPcAdminApproval
   - Validates rejection reason (required, max 3000 chars)
   - Changes status to RejectedByPcAdmin
   - Records rejection details
   - Adds rejection history entry

#### Queries & Handlers

**GetRequestByIdQuery**: Retrieve request details with all items, financial distributions, and history
- Returns RequestDto with complete hierarchy
- History ordered chronologically

**GetRequestsByStatusQuery**: Retrieve requests by status with full details
- Returns collection of RequestDto
- Includes history for all requests

#### DTOs
- **RequestDto**: Complete request data with history
- **RequestItemDto**: Item data with financial distributions
- **FinancialDistributionDto**: Distribution data
- **RequestHistoryDto**: History entry data ? NEW

### Infrastructure Layer (`PartnersHub.InfraBase.Infrastructure`)

#### DbContext
**InfrabaseDbContext**
- DbSets: Requests, RequestItems, FinancialDistributions, **RequestHistories** ?, Sectors, SubSectors, AssetTypes, UnitsOfMeasurement
- Automatically clears domain events after save
- Applies entity configurations from assembly

**UnitOfWork**
- Implements IUnitOfWork
- Wraps DbContext.SaveChangesAsync

#### Lookup Entities (? Soft Delete Removed)
- **Sector**: Name
- **SubSector**: Name, SectorId
- **AssetType**: Name
- **UnitOfMeasurement**: Name, Code

All lookups use hard delete (no IsDeleted flag).

#### Entity Configurations

**InfrabaseRequestConfiguration**
- Maps value objects (ProjectName, ProjectDescription, RejectionReason)
- Converts enums to strings
- Configures cascade delete for items and history
- Ignores domain events

**RequestItemConfiguration**
- Maps value objects (ItemName, Quantity, UnitPrice)
- Configures cascade delete for financial distributions

**FinancialDistributionConfiguration**
- Maps YearlyAmount value object
- Converts AmountType enum to string

**RequestHistoryConfiguration** ? NEW
- Maps all history fields
- Indexes on RequestId and PerformedAt for efficient querying
- Cascade delete with parent request

#### Repositories

**InfrabaseRequestRepository**
- Implements all query methods with proper EF Core includes
- **Always includes History** in all queries
- Auto-generates request codes (REQ-00001, REQ-00002, etc.)
- Auto-generates item codes (ITEM-00001, ITEM-00002, etc.)
- Case-insensitive project name uniqueness check

**LookupRepository**
- Validates existence of lookups
- **No soft delete checks** (uses hard delete)
- Validates sub-sector belongs to sector

## Business Rules Implemented

### Add Request Story
? User must have contributor or PC admin permissions
? Project name is required and unique
? Sector, sub-sector, and asset type are required
? Tendering stage and funding model are required
? Items can be added with unique names per request
? Item quantity and unit price must be positive
? Financial distributions (CAPEX/OPEX) required before submit
? Total distributions must equal item total
? Request can be saved as draft without validation
? Request generates code only on submit
? PC contributor submissions go to PC admin
? PC admin submissions go to Infrabase admin
? All actions are tracked in history

### PC Admin Approval/Rejection Story
? PC admin can view requests with status "PendingPcAdminApproval"
? PC admin can accept request
  - Confirmation required (handled at UI layer)
  - Changes status to "AcceptedByPcAdmin"
  - Records approval user and timestamp
  - Adds history entry
? PC admin can reject request
  - Confirmation required (handled at UI layer)
  - Rejection reason is mandatory (max 3000 characters)
  - Changes status to "RejectedByPcAdmin"
  - Records rejection details
  - Adds history entry with rejection reason

### History Tracking ? NEW
? Request creation tracked
? Draft saves tracked
? Submissions tracked with comments
? Basic information updates tracked with field-level changes
? Acceptances tracked
? Rejections tracked with reason
? All history includes: action, status, performer, timestamp
? History automatically included in all queries
? History ordered chronologically in responses

## Validation Summary

| Field | Validation |
|-------|------------|
| Project Name | Required, Unique, Max 500 chars |
| Project Description | Optional, Max 3000 chars |
| Sector | Required, Must exist (no soft delete) |
| Sub Sector | Required, Must exist, Must belong to sector (no soft delete) |
| Asset Type | Required, Must exist (no soft delete) |
| Tendering Stage | Required, Enum value |
| Funding Model | Required, Enum value |
| Item Name | Required, Unique per request, Max 500 chars |
| UOM | Required, Must exist (no soft delete) |
| Quantity | Required, Positive, Max 5 digits, No zero |
| Unit Price | Required, Positive, Max 10 digits, No zero |
| Financial Year | Required, 1-99, Integer |
| Yearly Amount | Required, Positive, Max 11 digits, No zero |
| Rejection Reason | Required (on reject), Max 3000 chars |

## Next Steps

1. **Create API Layer** (`PartnersHub.InfraBase.Apis`)
   - Controllers for requests
   - Authorization policies
   - Swagger documentation

2. **Add Migrations**
   - EF Core migrations for database schema

3. **Implement Remaining Stories**
   - Infrabase admin approval
   - Request viewing/listing with history
   - Request filtering and search
   - Audit logging enhancement

4. **Add Unit Tests**
   - Domain logic tests
   - Command handler tests
   - Repository tests
   - History tracking tests

5. **Add Integration Tests**
   - End-to-end API tests

## Dependencies

- .NET 8.0
- Entity Framework Core 8.0.0
- MediatR 13.0.0

## Database Tables

- **Requests**: Main request table
- **RequestItems**: Items within requests
- **FinancialDistributions**: CAPEX/OPEX distributions per item
- **RequestHistories**: Change history tracking ? NEW
- **Sectors**: Sector lookup (hard delete)
- **SubSectors**: Sub-sector lookup (hard delete)
- **AssetTypes**: Asset type lookup (hard delete)
- **UnitsOfMeasurement**: UOM lookup (hard delete)

## Key Features

? **No Soft Delete**: All lookups use hard delete for cleaner data management
? **Complete History Tracking**: Every action is recorded with details
? **Field-Level Change Tracking**: Updates show what changed, old values, and new values
? **Automatic History Inclusion**: History is always loaded with requests
? **Chronological Ordering**: History is always returned in time order
