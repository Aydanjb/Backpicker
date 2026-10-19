import { TableCell, TableRow } from '@/components/ui/table';
import type { Product, Slot } from '@/lib/types';
import { Button } from './ui/button';
import { IconPlus } from '@tabler/icons-react';

interface Props {
	slot: Slot;
	onSelect: (product: Product) => void;
}

function EmptyRow({ slot }: Props) {
	return (
		<TableRow>
			<TableCell className="capitalize underline">
				<a href="">{slot.label}</a>
			</TableCell>
			<TableCell>
				<Button>
					<IconPlus stroke={3} data-icon="inline-start" /> Add a{' '}
					{slot.label}
				</Button>
			</TableCell>
		</TableRow>
	);
}

export default EmptyRow;
