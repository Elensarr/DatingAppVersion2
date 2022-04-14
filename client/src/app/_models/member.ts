import { Photo } from "./photo";

export interface Member {
  id: number;
  username: string;
  photoUrl: string;
  age: number;
  knownAs: string;
  created: Date;
  lasrActive: Date;
  gender: string;
  interests: string;
  introduction: string;
  lookingFor: string;
  city: string;
  country: string;
  photos: Photo[];
}

