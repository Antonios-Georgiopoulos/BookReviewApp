# User Guide - Book Review App

Concise guide for interacting with the book review application.

## Getting Started

### For new users
1. **Registration**: "Register" button → Fill in details (email, username, password)
2. **Confirmation**: Automatic login after registration
3. **First use**: Browse available books

### Access rules
- **Visitors**: View books and reviews (read-only)
- **Registered users**: Full interaction (CRUD operations)

## Core Functions

### Book Management
```
Add → "Add Book" → Fill form
Edit → View book → "Edit"  
Delete → View book → "Delete" (with confirmation)
```

### Review System
- **Rating**: 1-5 stars (required)
- **Content**: 10-2000 characters
- **Limitation**: One review per book per user
- **Editing**: Only by creator

### Voting (Like/Dislike)
- **Rules**: One vote per review, not on your own reviews
- **Change vote**: Click opposite button
- **Remove**: Click same button

## Search & Filtering

### Available filters
- **Search**: Title or author
- **Genre**: Dropdown with all available genres
- **Year**: Publication year
- **Rating**: Minimum rating (1-5 stars)
- **Sort**: Title, author, year, rating, date

### Search tips
- Use partial words for better results
- Filters combine (AND logic)
- "Clear" to reset all filters

## Profile Management

### Access
Username (top right) → "Manage"

### Available options
- Change password
- Update email
- View personal data

## Frequently Asked Questions

**Q: Can I change my review?**
A: Yes, through the menu (⋮) on your review.

**Q: How many reviews can I write for one book?**
A: Only one per book.

**Q: Why can't I vote on a review?**
A: You cannot vote on your own reviews or if you've already voted.

**Q: How do I delete my account?**
A: Contact the administrator via email.

## Interface Tips

### Quick actions
- **Enter** in search for immediate execution
- **Click** on book title for details view
- **Hover** on stars for rating preview

### Responsive design
- Application adapts to all devices
- Mobile-first navigation in hamburger menu
- Touch-friendly buttons and forms

## Technical Support

For technical issues or bug reports:
- **Email**: support@bookreviewapp.com  
- **Describe**: The problem and reproduction steps