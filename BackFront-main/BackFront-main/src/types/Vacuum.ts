import type { CardInfoBase } from './CardInfoBase';

export interface Vacuum extends CardInfoBase {
    brand: string;
    price: number;
    power: number;
    description: string;
}