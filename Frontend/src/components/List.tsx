import { type JSX, useState } from 'react';
import {
	Table,
	TableBody,
	TableCell,
	TableFooter,
	TableHead,
	TableHeader,
	TableRow,
} from '@/components/ui/table';
import ListRow from './ListRow';
import type { Kit, Product, Slot } from '@/lib/types.tsx';
import { formatPrice, formatWeight, parseOunces } from '@/lib/utils.ts';

type Props = {
	className?: string;
};

function List({ className }: Props): JSX.Element {
	const [kit, _setKit] = useState<Kit>({
		Backpack: {
			id: 0,
			name: "Osprey Stratos 24 Pack - Men's",
			priceCents: 14999,
			weights: ['2 lbs. 15.2 oz.'],
			link: 'https://www.rei.com/product/C03941/osprey-stratos-24-pack-mens',
			image: 'https://www.rei.com/media/3c00cd80-0f87-400f-9fc0-0bc7d78d033b',
		},
	});

	const slots: Slot[] = [
		{ id: 1, label: 'Backpack' },
		{ id: 2, label: 'Shelter' },
		{ id: 3, label: 'Sleeping Bag' },
		{ id: 4, label: 'Sleeping Pad' },
		{ id: 5, label: 'Pillow' },
	];

	const products = Object.values(kit).filter((p): p is Product => !!p);
	const totals = products.reduce(
		(acc, p) => ({
			priceCents: acc.priceCents + p.priceCents,
			ounces: acc.ounces + parseOunces(p.weights),
		}),
		{ priceCents: 0, ounces: 0 }
	);

	return (
		<Table className={className}>
			<TableHeader>
				<TableRow>
					<TableHead>Item</TableHead>
					<TableHead>Selection</TableHead>
					<TableHead>Weight</TableHead>
					<TableHead>Price</TableHead>
					<TableHead>Where</TableHead>
				</TableRow>
			</TableHeader>
			<TableBody>
				{slots.map((slot) => (
					<ListRow
						key={slot.id}
						slot={slot}
						product={kit[slot.label]}
						onSelect={() => 'a'}
						onRemove={() => 'b'}
					/>
				))}
			</TableBody>
			<TableFooter>
				<TableRow className="text-xl font-bold">
					<TableCell colSpan={2}>Total</TableCell>
					<TableCell>{formatWeight(totals.ounces)}</TableCell>
					<TableCell>{formatPrice(totals.priceCents)}</TableCell>
					<TableCell />
				</TableRow>
			</TableFooter>
		</Table>
	);
}

export default List;
