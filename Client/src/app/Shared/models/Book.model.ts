export interface Book {
  id: number;
  title: string;
  author: string;
  publishedDate: Date| string;
  price: number;
  availableCopies: number;
  genreID: number;
  genre?: Genre | null;
  image: string;
  imageFile?: File | null;
  isAvailable: boolean;
}

export interface Genre {
  genreID: number;
  genreName: string;
  description: string;
}
