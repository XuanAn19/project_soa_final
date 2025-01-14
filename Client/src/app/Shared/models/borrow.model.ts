// src/app/models/book.dto.ts
export interface BookDTO {
  id: number;
  title: string;
  availableCopies: number;
}

// src/app/models/user.dto.ts
export interface UserDTO {
  id: string;
  fullName: string;
}

// src/app/models/borrow.model.ts
export interface BorrowModel {
  id: number;
  bookId: number;
  bookName: string;
  userId: string;
  fullName: string;
  isTrue: boolean;
  borrowDate: Date;
  returnDate?: Date;
}
