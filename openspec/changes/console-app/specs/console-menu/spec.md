## ADDED Requirements

### Requirement: Menu supports unlimited nesting depth
The menu system SHALL support creating submenus at any depth. Each `MenuItem` MAY contain a child `Menu` that becomes the active menu when selected.

#### Scenario: Navigate into a submenu
- **WHEN** the user selects a menu item that has a child submenu
- **THEN** the display switches to the child menu's items

#### Scenario: Three levels of nesting
- **WHEN** a menu tree has Main → Settings → Player Config
- **THEN** the user can navigate from Main into Settings and from Settings into Player Config

### Requirement: Level-aware navigation items
The menu SHALL automatically provide navigation items based on the current menu depth.

#### Scenario: Level 1 menu shows Exit only
- **WHEN** the active menu is at level 1 (root)
- **THEN** the only navigation item is "Exit"

#### Scenario: Level 2 menu shows Exit and Return to Previous
- **WHEN** the active menu is at level 2
- **THEN** navigation items include "Exit" and "Return to Previous"

#### Scenario: Level 3+ menu shows Exit, Return to Previous, and Return to Main
- **WHEN** the active menu is at level 3 or deeper
- **THEN** navigation items include "Exit", "Return to Previous", and "Return to Main"

### Requirement: Cursor-based selection
The menu SHALL support arrow key navigation with a visible cursor indicator on the currently highlighted item.

#### Scenario: Move cursor down
- **WHEN** the user presses the Down arrow key
- **THEN** the cursor moves to the next menu item (wrapping to first if at the end)

#### Scenario: Move cursor up
- **WHEN** the user presses the Up arrow key
- **THEN** the cursor moves to the previous menu item (wrapping to last if at the start)

#### Scenario: Select item with Enter
- **WHEN** the user presses Enter on a highlighted item
- **THEN** the item's action callback executes

### Requirement: Number key shortcuts
The menu SHALL allow selecting items by pressing the corresponding number key (1-based index).

#### Scenario: Press number key to select
- **WHEN** the user presses a digit key that matches a visible menu item's position
- **THEN** that item's action callback executes immediately

#### Scenario: Invalid number key is ignored
- **WHEN** the user presses a digit key that does not correspond to any menu item
- **THEN** nothing happens and the menu remains displayed

### Requirement: Updateable menu item labels
Menu items SHALL support dynamic labels that refresh each time the menu is displayed. This allows showing current values (e.g., "Board Width [7]").

#### Scenario: Label reflects current value
- **WHEN** a menu item has a dynamic label function and the underlying value changes from 7 to 10
- **THEN** the menu displays the updated label (e.g., "Board Width [10]") on the next render

### Requirement: Menu item action callbacks
Each menu item SHALL accept a `Func<string>` delegate that executes when the item is selected. The returned string MAY be displayed as feedback.

#### Scenario: Action executes on selection
- **WHEN** the user selects a menu item
- **THEN** the item's action delegate is invoked

### Requirement: Hot key uniqueness validation
The menu SHALL reject registration of menu items with duplicate shortcut keys within the same menu level.

#### Scenario: Duplicate shortcut rejected
- **WHEN** a menu item is added with a shortcut key that already exists in the same menu
- **THEN** an exception is thrown at registration time
