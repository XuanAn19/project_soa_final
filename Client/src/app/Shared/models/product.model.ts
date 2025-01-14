export interface Products {
  count: number;
  products: Product[];
}

export interface Product {
  id: Number;
  name: String;
  description: String;
  price: Number;
  quantity: Number;
  created_day : Date;
  updated_day :Date
}
