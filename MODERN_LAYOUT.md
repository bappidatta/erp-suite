# Modern Dashboard Layout - Architecture & Features

## 🎨 Overview

A beautiful, responsive, and modern dashboard layout built with shadcn/ui and Tailwind CSS v4. The layout features a professional sidebar navigation, dynamic header, and responsive design that works seamlessly on desktop and mobile devices.

## 📁 File Structure

```
src/app/components/layout/
├── DashboardLayout.tsx      # Main layout wrapper
├── Header.tsx               # Top navigation bar with user menu
├── Sidebar.tsx              # Desktop & mobile navigation sidebar
```

## 🏗️ Architecture

### DashboardLayout Component
The **root layout wrapper** that combines header and sidebar for all authenticated pages.

**Features:**
- Flexbox-based responsive layout
- Hidden sidebar on mobile (<lg screens)
- Sticky header at top
- Scrollable main content area
- Full-screen dashboard (100vh)

```tsx
<DashboardLayout>
  <YourPage />
</DashboardLayout>
```

**Output Structure:**
```
┌─────────────────────────────────┐
│         Header (sticky)          │
├──────────┬──────────────────────┤
│          │                      │
│ Sidebar  │   Main Content       │
│ (hidden) │   (scrollable)       │
│ on mobile│                      │
│          │                      │
└──────────┴──────────────────────┘
```

### Header Component
**Location:** [src/app/components/layout/Header.tsx](../layout/Header.tsx)

**Features:**
- Mobile menu trigger (Sheet drawer)
- Search bar (hidden on mobile)
- Notification bell with unread indicator
- User profile dropdown
- Logout button
- Sticky positioning

**UI Elements:**
```
[Menu]  [Search...]        [🔔]  [User Avatar ▼]
```

**User Dropdown Menu:**
- User profile info (name, email, avatar)
- Profile link
- Settings link
- Logout option (red accent)

### Sidebar Component
**Location:** [src/app/components/layout/Sidebar.tsx](../layout/Sidebar.tsx)

**Features:**
- Desktop: Always visible sidebar (64 width)
- Mobile: Collapsible Sheet drawer
- Expandable/collapsible menu sections
- Active nav item highlighting
- Icons from lucide-react
- Scrollable navigation area
- Settings & Logout in footer

**Navigation Structure:**
```
┌─ ERP Suite
├─ 🏠 Dashboard → /dashboard
├─ 👥 Admin
│  ├─ Users → /admin/users
│  ├─ Roles → /admin/roles
│  └─ Companies → /admin/companies
├─ 📄 Finance
│  ├─ Invoices → /finance/invoices
│  ├─ Expenses → /finance/expenses
│  └─ Reports → /finance/reports
├─ 📦 Procurement
│  ├─ Purchase Orders → /procurement/orders
│  ├─ Vendors → /procurement/vendors
│  └─ Requisitions → /procurement/requisitions
├─ ⚙️ Settings
└─ 🚪 Logout
```

### Dashboard Page
**Location:** [src/app/DashboardPage.tsx](../DashboardPage.tsx)

**Updated with:**
- Welcome greeting with user's full name
- Key metrics cards with trend indicators
- Recent transactions table via DashboardGrid component
- Beautiful gradient backgrounds
- Responsive grid layout

**Metric Cards:**
- Total Revenue ($45,231) with +12.5% trend
- Active Users (1,234) with +8% trend
- Pending Invoices ($12,500) with 5 due
- System Status (99.9%) Operational

## 🎯 Key Components Used

### shadcn/ui Components
- **Button** - Styled buttons with variants
- **Card** - Content containers
- **Input** - Search & form fields
- **Label** - Form labels
- **Select** - Dropdowns
- **Table** - Data grids
- **Badge** - Status indicators
- **Dialog** - Modals
- **DropdownMenu** - Context menus
- **Sheet** - Mobile drawer
- **ScrollArea** - Scrollable containers
- **Separator** - Visual dividers
- **Collapsible** - Expandable sections
- **Alert** - Notifications

### Icons
- **lucide-react** (200+ icons)
  - LayoutDashboard, Users, FileText, Settings
  - Menu, X, Bell, Search, LogOut, ChevronDown
  - ArrowUpRight, ArrowDownRight

## 💫 UI Features

### Color System
- **Primary**: Blue (#1f5eff) - Used for active nav items
- **Background**: Light (#f7f5ef)
- **Accent**: Subtle hover states
- **Destructive**: Red for logout
- CSS variables: `--primary`, `--background`, `--foreground`, etc.

### Typography
- **H1**: 3xl font-bold (Welcome heading)
- **H2**: 2xl font-bold (Card titles)
- **Metadata**: text-xs text-muted-foreground

### Responsive Design
```
📱 Mobile (<768px)
- Full-width content
- Menu in Sheet drawer
- No sidebar
- Compact header

💻 Desktop (≥768px)
- Search bar visible
- Sidebar always visible (64px wide)
- Full spacing

🖥️ Large (≥1024px)
- All features enabled
- Maximum content width
- Comfortable spacing
```

## 🔄 Integration Points

### ProtectedRoute Wrapper
```tsx
// Before: Just <Outlet />
// After: Wrapped with DashboardLayout
<ProtectedRoute>
  <DashboardLayout>
    <Outlet />
  </DashboardLayout>
</ProtectedRoute>
```

All authenticated pages now automatically get:
- Header with navigation
- Sidebar with menu
- Proper styling and spacing

### Authentication Context
Uses `auth.user` from AuthContext:
```tsx
const { auth, logout } = useAuth();

auth?.user = {
  id: number,
  email: string,
  fullName: string
}
```

## 🎨 Styling

### Tailwind CSS v4
- CSS variables for colors
- OKLch color space (modern, perceptually uniform)
- Dark mode support via `data-[mode=dark]`
- Smooth animations with @tailwindcss/animate

### Custom CSS Classes
From globals.css:
```css
@tailwind base;
@tailwind components;
@tailwind utilities;

/* Variables */
--background: oklch(1 0 0)
--foreground: oklch(0.145 0 0)
--primary: oklch(0.205 0 0)
--destructive: oklch(0.58 0.22 27)
/* ... more colors ... */
```

## 📱 Mobile Responsiveness

### Hidden Elements on Mobile
- Sidebar (shown in Sheet drawer via menu button)
- Search bar (can be accessed via search icon)
- User name (shown as initials in avatar)

### Breakpoints Used
```tsx
// Tailwind breakpoints
hidden md:flex  // hidden on mobile, flex on 768px+
hidden lg:block // hidden on mobile/tablet, block on 1024px+
-based hidden lmd:hidden // shown on mobile, hidden on 768px+
```

## 🚀 Performance

### Build Output
- CSS: ~41 KB (7.3 KB gzipped)
- JS: ~461 KB (148 KB gzipped)
- Total: Optimized with code splitting

### Features
- Lazy component loading
- Optimized icons (tree-shakeable)
- CSS variables for theming
- No unnecessary re-renders

## 📋 Available Modules

The sidebar includes navigation to future modules:

### ✅ Implemented
- **Dashboard** - Landing page with metrics

### 🔄 In Progress
- **Admin** - User & role management
- **Finance** - GL, AP, AR modules
- **Procurement** - PO, requisition workflows

## 🔐 Security

- Protected routes require authentication
- Logout clears auth state and localStorage
- User info displayed from verified auth context
- No sensitive data in localStorage except token

## 🎓 Development Notes

### Adding New Sidebar Items
Edit `navItems` in [Sidebar.tsx](../layout/Sidebar.tsx):

```tsx
const navItems = [
  {
    label: "New Module",
    href: "/new-module",  // for single pages
    icon: IconComponent,
    submenu: [            // optional sub-items
      { label: "Sub Item", href: "/new-module/sub" }
    ]
  },
  // ...
];
```

### Customizing Colors
Edit CSS variables in [src/styles/globals.css](../styles/globals.css):

```css
--primary: oklch(...);
--background: oklch(...);
```

### Adding Components
New shadcn/ui components can be added:
```bash
npx shadcn@latest add component-name
```

## 📚 Component Hierarchy

```
App
├─ LoginPage (public)
└─ ProtectedRoute
   └─ DashboardLayout
      ├─ Sidebar (desktop)
      ├─ Header
      │  └─ MobileSidebar (sheet)
      └─ Main Content (pages)
         ├─ DashboardPage
         ├─ UsersPage
         ├─ InvoicesPage
         └─ ...
```

## 🎯 Next Steps

1. **Implement module pages** - Create pages for each navigation item
2. **Add user management** - Users CRUD interface
3. **Finance module** - Invoice and expense management
4. **Procurement module** - PO and requisition workflows
5. **Dark mode** - Add theme toggle in header
6. **Mobile optimization** - Fine-tune mobile UX

## 📦 Recent Commits

- **1702912** - feat: create beautiful modern dashboard layout with shadcn/ui
- **b3cefd1** - feat: add essential shadcn/ui components (card, form, table, dialog, etc.)
- **25058c4** - feat: install and configure shadcn/ui with Tailwind CSS v4

---

**Status:** ✅ Production-ready layout with professional design
**Last Updated:** March 8, 2026
